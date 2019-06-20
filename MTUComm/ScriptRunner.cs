﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Lexi.Interfaces;
using Library;
using Library.Exceptions;
using Xml;
using Library;

using ActionType = MTUComm.Action.ActionType;

namespace MTUComm
{
    public class ScriptRunner
    {
        private List<Action> actions;

        public delegate void ActionFinishHandler(object sender, Action.ActionFinishArgs e);
        public event ActionFinishHandler OnFinish;

        public delegate void ActionOnProgressHandler(object sender, Action.ActionProgressArgs e);
        public event ActionOnProgressHandler OnProgress;

        public delegate void ActionStepFinishHandler(object sender, int step, Action.ActionFinishArgs e);
        public event ActionStepFinishHandler onStepFinish;

        public delegate void ActionErrorHandler ();
        public event ActionErrorHandler OnError;

        public void ParseScriptAndRun ( ISerial serial_device, String script_stream, int stream_size )
        {
            XmlSerializer s = null;
        
            try
            {
                // Script file is empty
                if ( string.IsNullOrEmpty ( script_stream.Trim () ) )
                    throw new ScriptEmptyException ();
            
                Script script = new Script ();
                s = new XmlSerializer ( typeof ( Script ) );
    
                // Register unknown elements ( not present in Script class ) as additional parameters
                s.UnknownElement += this.UnknownElementEvent;
            
                using ( StringReader reader = new StringReader ( script_stream.Substring ( 0, stream_size ) ) )
                {
                    script = ( Script )s.Deserialize ( reader );
                }
                BuildScriptActions ( serial_device, script );
            }
            catch ( Exception e )
            {
                if ( ! Errors.IsOwnException ( e ) )
                     Errors.LogErrorNowAndContinue ( new ScriptWrongStructureException () ); // Script file has invalid format or structure
                else Errors.LogErrorNowAndContinue ( e ); // ScriptLogfileInvalidException, ScriptActionTypeInvalidException

                this.OnError ();
                
                return;
            }
            finally
            {
                s.UnknownElement -= this.UnknownElementEvent;
            }

            this.Run ();
        }

        private void UnknownElementEvent ( object sender, XmlElementEventArgs e )
        {
            ScriptAction script = ( ScriptAction )e.ObjectBeingDeserialized;
            script.AddAdditionParameter ( e.Element.Name, e.Element.InnerText );
        }

        private void BuildScriptActions ( ISerial serial_device, Script script )
        {
            actions = new List<Action>();

            int step = 0;

            if ( string.IsNullOrEmpty ( script.UserName ) )
                throw new ScriptUserNameMissingException ();

            Mobile.LogUserPath = script.UserName;

            // Using invalid log file/path
            if ( string.IsNullOrEmpty ( script.LogFile ) ||
                 ! Regex.IsMatch ( script.LogFile, @"^[a-zA-Z_][a-zA-Z0-9_-]*.xml$" ) )
                throw new ScriptLogfileInvalidException ();

            else
            {
                ActionType type;
                foreach ( Xml.ScriptAction scriptAction in script.Actions )
                {
                    // Action string is not present in ActionType enum
                    if ( ! Enum.TryParse<ActionType> ( scriptAction.Type, out type ) )
                        throw new ScriptActionTypeInvalidException ( scriptAction.Type );
                
                    Action new_action = new Action ( Singleton.Get.Configuration, serial_device, type, script.UserName, script.LogFile );
                    Type   actionType = scriptAction.GetType ();
    
                    Parameter.ParameterType paramTypeToAdd;
                    foreach ( PropertyInfo propertyInfo in scriptAction.GetType().GetProperties () )
                    {
                        var property   = actionType.GetProperty ( propertyInfo.Name );
                        var paramValue = property.GetValue ( scriptAction, null );
                        if ( paramValue is null )
                            continue;
                        
                        Type valueType = paramValue.GetType ();
                    
                        if ( valueType.Name.ToLower ().Contains ( "actionparameter" ) )
                        {
                            // The parameter name is not listed in the ParameterType enumeration
                            if ( ! Enum.TryParse<Parameter.ParameterType> ( propertyInfo.Name, out paramTypeToAdd ) )
                                continue;
                        
                            List<ActionParameter> list = new List<ActionParameter> ();
                            
                            // If the parameter is an Array is a field for
                            // a port, and if not is a field for the MTU
                            if ( ! paramValue.GetType ().IsArray )
                                 list.Add      ( ( ActionParameter   )paramValue );
                            else list.AddRange ( ( ActionParameter[] )paramValue );

                            foreach ( ActionParameter aParam in list )
                                new_action.AddParameter (
                                    new Parameter (
                                        paramTypeToAdd,
                                        aParam.Value,
                                        aParam.Port ) );
                        }
                    }

                    // Additional parameters
                    Parameter parameter;
                    foreach ( var entry in scriptAction.AdditionalParameters )
                    {
                        parameter = new Parameter ( entry.Key, entry.Key, entry.Value );
                        parameter.Optional = true;
                        new_action.AddAdditionalParameter ( parameter );
                    }
    
                    new_action.order = step;
                    new_action.OnProgress += Action_OnProgress;
                    new_action.OnFinish += Action_OnFinish;
                    new_action.OnError += Action_OnError;
    
                    actions.Add(new_action);
                    step++;
                }
            }
        }

        public void Run()
        {
            actions.ToArray()[0].Run();
        }

        private void Action_OnError ()
        {
            this.OnError ();
        }

        private void Action_OnProgress(object sender, Action.ActionProgressArgs e)
        {
            OnProgress(sender, e);
        }

        private void Action_OnFinish(object sender, Action.ActionFinishArgs e)
        {
            Action act = (Action)sender;
            if (act.order < (actions.Count-1))
            {
                onStepFinish(act, act.order, e);
                actions.ToArray()[act.order+1].Run();
            }
            else
            {
                OnFinish(act, e);
            }
        }
    }
}
