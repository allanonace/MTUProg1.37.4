﻿using System;
using System.Windows.Input;
using Xamarin.Forms;
using aclara_meters.Models;
using aclara_meters.Helpers;
using aclara_meters.view;
using Acr.UserDialogs;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Library;

using MTUComm;
using System.Linq;
using System.Collections.Generic;

namespace aclara_meters.viewmodel
{
    public class LoginMenuViewModel : ViewModelBase
    {
        private const string AES_KEY = "SOLONROCKSACLARA";
    
        #region Commands
        public INavigation Navigation { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        #endregion

        #region Properties
        private User _user = new User();

        public User User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }
        #endregion

        IUserDialogs dialogs_save;

        public LoginMenuViewModel(IUserDialogs dialogs)
        {
            dialogs_save = dialogs;
            LoginCommand = new Command(Login);
            LoadCommand = new Command(Load);
            Task.Run(async () =>
            {
                await Task.Delay(550); Device.BeginInvokeOnMainThread(() =>
                {
                    LoadCommand.Execute(null);
                });
            });
        }

        private void PathsLogs(string user)
        {
            Mobile.LogPath = Mobile.ConfigPublicPath;//public folder
            Mobile.LogUserBackupPath = user;  //public folder
            if (FormsApp.config.Global.LogsPublicDir)
                Mobile.LogUserPath = user;
            else
            {
                Mobile.LogPath = Mobile.ConfigPath;  // private folder
                Mobile.LogUserPath = user;
            }
        }

        public async void Load ()
        {
            if ( await FormsApp.credentialsService.CredentialsExist () )
            {
                string user = FormsApp.credentialsService.UserName;
                //Mobile.LogUserPath = user;

                PathsLogs(user);

                FormsApp.logger.Login(user);
                Settings.IsLoggedIn = true;
                Application.Current.MainPage=new NavigationPage(new AclaraViewMainMenu(dialogs_save));
            }   
        }

        private bool AreCredentialsCorrect (
            string username,
            string password )
        {
            Xml.User[] dbUsers = Singleton.Get.Configuration.users;
            
            IEnumerable<Xml.User> coincidences = dbUsers.Where ( user => user.Name.Equals ( username ) );
            if ( coincidences.Count () == 1 )
            {
                Xml.User dbUser = coincidences.ToList<Xml.User> ()[ 0 ];
                
                if ( dbUser.Encrypted )
                    return Utils.EncryptStringToBase64_Aes ( password, AES_KEY ).Equals ( dbUser.Pass );
                return password.Equals ( dbUser.Pass );
            }
            
            return false;
        }

        public async void Login()
        {
            IsBusy = true;
            Title = string.Empty;
            try
            {
                if (User.Email != null)
                {
                    if (User.Password != null)
                    {
                        string userName = User.Email;
                        string password = User.Password;

                        #region Credentials length Validation

                        if (userName.Length < FormsApp.config.Global.UserIdMinLength || userName.Length > FormsApp.config.Global.UserIdMaxLength)
                        {
                            IsBusy = false;
                            Message = "The field UserName must be with a minimum length of " + FormsApp.config.Global.UserIdMinLength+ " and a maximum length of " + FormsApp.config.Global.UserIdMaxLength;
                            return;
                        }

                        if(password.Length < FormsApp.config.Global.PasswordMinLength || password.Length > FormsApp.config.Global.PasswordMaxLength)
                        {
                            IsBusy = false;
                            Message = "The field Password must be with a minimum length of " + FormsApp.config.Global.PasswordMinLength + " and a maximum length of " + FormsApp.config.Global.PasswordMaxLength;
                            return;
                        }

                        #endregion

                        var isValid = AreCredentialsCorrect(userName, password);

                        if (isValid)
                        {
                            if ( ! await FormsApp.credentialsService.CredentialsExist () )
                                await FormsApp.credentialsService.SaveCredentials ( userName, password );
                            
                            Settings.IsLoggedIn = true;
                            Settings.SavedUserName = User.Email;

                            PathsLogs(userName);

                            FormsApp.logger.Login(userName);
                            //await Application.Current.MainPage.Navigation.PushAsync(new AclaraViewMainMenu(dialogs_save), false);
                            Application.Current.MainPage = new NavigationPage(new AclaraViewMainMenu(dialogs_save));
                            //await Application.Current.MainPage.Navigation.PushAsync(new AclaraViewGlobalUIController(), false);
                        }
                        else
                        {
                            Message = "Wrong username or password";
                        }

                        IsBusy = false;
                    }
                    else
                    {
                        IsBusy = false;
                        Message = "Password required";
                    }
                }
                else
                {
                    IsBusy = false;
                    Message = "Email required";
                }

            }
            catch (Exception e)
            {
                IsBusy = false;
                await Application.Current.MainPage.DisplayAlert("Connection error", e.Message, "Ok");
            }
        }
    }
}