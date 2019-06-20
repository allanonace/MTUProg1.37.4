﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Xml
{
    public class Mtu
    {
        public enum VERSION { NEW, ARCH };
    
        private const int DEF_FLOW = 0;
    
        public Mtu ()
        {
            //this.Description              = SET BY ACLARA
            //this.Flow                     = SET BY ACLARA [0,1]
            //this.IsEcoder                 = Konstantin: Please use Ecoder only,  isEcoder is for legacy MTUs
            //this.MagneticTamper           = ¿?
            //this.Model                    = SET BY ACLARA

            this.CutWireDelaySetting        = false;
            this.DataRead                   = false;
            this.DailyReads                 = true;
            this.DigitsToDrop               = false;
            this.Ecoder                     = false;     // Ecoder Only MTUs
            this.ECoderDaysNoFlow           = false;
            this.ECoderDaysOfLeak           = false;
            this.ECoderLeakDetectionCurrent = false;
            this.ECoderReverseFlow          = false;
            this.FastMessageConfig          = false;
            this.GasCutWireAlarm            = false;
            this.GasCutWireAlarmImm         = false;
            this.InsufficientMemory         = false;
            this.InsufficientMemoryImm      = false;
            this.InterfaceTamper            = false;
            this.InterfaceTamperImm         = false;
            this.LastGasp                   = false;
            this.LastGaspImm                = false;
            this.MtuDemand                  = false;
            this.NodeDiscovery              = false;
            this.OnTimeSync                 = false;
            this.PulseCountOnly             = false;
            this.RegisterCoverTamper        = false;
            this.RequiresAlarmProfile       = false;
            this.ReverseFlowTamper          = false;
            this.SerialComProblem           = false;
            this.SerialComProblemImm        = false;
            this.SerialCutWire              = false;
            this.SerialCutWireImm           = false;
            this.STAREncryptionType         = "AES256";  // [None,AES128,AES256]
            this.SpecialSet                 = false;
            this.TamperPort1                = false;
            this.TamperPort2                = false;
            this.TamperPort1Imm             = false;
            this.TamperPort2Imm             = false;
            this.TiltTamper                 = false;
            this.TimeToSync                 = false;
        }

        #region Elements

        [XmlAttribute("ID")]
        public int Id { get; set; }

        [XmlElement("DataRead")]
        public bool DataRead { get; set; }
    
        [XmlElement("DailyReads")]
        public bool DailyReads { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlElement("DigitsToDrop")]
        public bool DigitsToDrop { get; set; }

        [XmlElement("Ecoder")]
        public bool Ecoder { get; set; }

        [XmlElement("ECoderDaysNoFlow")]
        public bool ECoderDaysNoFlow { get; set; }

        [XmlElement("ECoderDaysOfLeak")]
        public bool ECoderDaysOfLeak { get; set; }

        [XmlElement("ECoderLeakDetectionCurrent")]
        public bool ECoderLeakDetectionCurrent { get; set; }

        [XmlElement("ECoderReverseFlow")]
        public bool ECoderReverseFlow { get; set; }

        [XmlElement("FastMessageConfig")]
        public bool FastMessageConfig { get; set; }
        
        [XmlIgnore]
        public int Flow { get; set; }
        
        [XmlElement("Flow")]
        public string Flow_AllowEmptyField
        {
            get { return this.Flow.ToString (); }
            set
            {
                if ( ! string.IsNullOrEmpty ( value ) )
                {
                    int v;
                    if ( int.TryParse ( value, out v ) )
                         this.Flow = v;
                    else this.Flow = DEF_FLOW;
                }
                else this.Flow = DEF_FLOW;
            }
        }

        [XmlElement("HexNum")]
        public string HexNum { get; set; }
        
        [XmlIgnore]
        public bool IsFamilly31xx32xx
        {
            get
            {
                string hexnum = this.HexNum.ToLower ();
            
                return hexnum.StartsWith ( "31" ) ||
                       hexnum.StartsWith ( "32" );
            }
        }
        
        [XmlIgnore]
        public bool IsFamilly33xx
        {
            get { return this.HexNum.ToLower ().StartsWith ( "33" ); }
        }
        
        [XmlIgnore]
        public bool IsFamilly342x
        {
            get { return this.HexNum.ToLower ().StartsWith ( "342" ); }
        }

        [XmlElement("Model")]
        public string Model { get; set; }

        [XmlElement("MtuDemand")]
        public bool MtuDemand { get; set; }
        
        [XmlElement("NodeDiscovery")]
        public bool NodeDiscovery { get; set; }

        [XmlElement("OnTimeSync")]
        public bool OnTimeSync { get; set; }

        [XmlElement("Port")]
        public List<Port> Ports { get; set; }
        
        [XmlIgnore]
        public Port Port1
        {
            get { return this.Ports[ 0 ]; }
        }
        
        [XmlIgnore]
        public Port Port2
        {
            get { return this.Ports[ 1 ]; }
        }
        
        [XmlElement("PulseCountOnly")]
        public bool PulseCountOnly { get; set; }
        
        [XmlElement("RequiresAlarmProfile")]
        public bool RequiresAlarmProfile { get; set; }
        
        [XmlElement("STAREncryptionType")]
        public string STAREncryptionType { get; set; }
        
        [XmlElement("SpecialSet")]
        public bool SpecialSet { get; set; }

        [XmlElement("TimeToSync")]
        public bool TimeToSync { get; set; }
        
        [XmlElement("Version")]
        public string VersionString { get; set; }

        #region Tampers

        [XmlElement("CutWireDelaySetting")]
        public bool CutWireDelaySetting { get; set; }
        
        [XmlElement("GasCutWireAlarm")]
        public bool GasCutWireAlarm { get; set; }
        
        [XmlElement("GasCutWireAlarmImm")]
        public bool GasCutWireAlarmImm { get; set; }
        
        [XmlElement("InsufficentMemory")]
        public bool InsufficientMemory { get; set; }

        [XmlElement("InsufficentMemoryImm")]
        public bool InsufficientMemoryImm { get; set; }
        
        [XmlElement("InterfaceTamper")]
        public bool InterfaceTamper { get; set; }
        
        [XmlElement("InterfaceTamperImm")]
        public bool InterfaceTamperImm { get; set; }
        
        [XmlElement("LastGasp")]
        public bool LastGasp { get; set; }
        
        [XmlElement("LastGaspImm")]
        public bool LastGaspImm { get; set; }
        
        [XmlElement("MagneticTamper")]
        public bool MagneticTamper { get; set; }
        
        [XmlElement("RegisterCoverTamper")]
        public bool RegisterCoverTamper { get; set; }
        
        [XmlElement("ReverseFlowTamper")]
        public bool ReverseFlowTamper { get; set; }
        
        [XmlElement("SerialComProblem")]
        public bool SerialComProblem { get; set; }
        
        [XmlElement("SerialComProblemImm")]
        public bool SerialComProblemImm { get; set; }
        
        [XmlElement("SerialCutWire")]
        public bool SerialCutWire { get; set; }

        [XmlElement("SerialCutWireImm")]
        public bool SerialCutWireImm { get; set; }
        
        [XmlElement("TamperPort1")]
        public bool TamperPort1 { get; set; }

        [XmlElement("TamperPort2")]
        public bool TamperPort2 { get; set; }

        [XmlElement("TamperPort1Imm")]
        public bool TamperPort1Imm { get; set; }

        [XmlElement("TamperPort2Imm")]
        public bool TamperPort2Imm { get; set; }
        
        [XmlElement("TiltTamper")]
        public bool TiltTamper { get; set; }

        #endregion

        #endregion

        #region Logic

        public String GetProperty ( String Name )
        {
            return this.GetType ().GetProperty ( Name ).GetValue ( this, null ).ToString ();
        }

        [XmlIgnore]
        public bool TwoPorts
        {
            get
            {
                return ( this.Ports.Count > 1 );
            }
        }
        
        [XmlIgnore]
        public VERSION Version
        {
            get { return ( this.VersionString.ToLower ().Equals ( VERSION.NEW.ToString ().ToLower () ) ) ? VERSION.NEW : VERSION.ARCH; }
        }
        
        [XmlIgnore]
        public bool IsArchVersion
        {
            get { return this.Version == VERSION.ARCH; }
        }

        [XmlIgnore]
        public bool IsNewVersion
        {
            get { return this.Version == VERSION.NEW; }
        }

        #endregion
    }
}
