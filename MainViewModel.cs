using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using System.Threading;
using System.Windows;

namespace LightChatting_GUI
{
    public class MainViewModel : ViewModelBase
    {
        public CommandBase CreateChatingCommand => new( CreateChatting );
        public CommandBase JoinChattingCommand => new( JoinChatting );
        public CommandBase ShowAboutCommand => new( ShowAboutPage );

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty( ref _name, value );
        }

        private SettingDialogViewModel createSetting;
        private SettingDialogViewModel joinSetting;
        private SettingDialogView createSettingView = new();
        private SettingDialogView joinSettingView = new();

        private void ClosingEventHandler( object sender, DialogClosingEventArgs eventArgs )
        {
            // Nothing to do.
        }
        private async void CreateChatting( object? _ )
        {
            //show the dialog
            var result = await DialogHost.Show( createSettingView, 
                "SettingDialog", ClosingEventHandler );
            if ( result.ToString() == "Accept" && 
                createSettingView.setting.CanChatting )
            {
                createSetting = createSettingView.setting;
                Name = createSettingView.setting.DefaultName;
                ChattingView chattingView = new( ref createSetting, 0 );
                chattingView.ShowDialog();
            }

        }

        private async void JoinChatting( object? _ )
        {
            //show the dialog
            if ( Name != string.Empty )
            {
                joinSettingView.setting.DefaultName = Name;
            }
            var result = await DialogHost.Show( joinSettingView, "SettingDialog", ClosingEventHandler );
            if ( result.ToString() == "Accept" &&
                joinSettingView.setting.CanChatting )
            {
                if ( Name == null )
                {
                    Name = joinSettingView.setting.DefaultName;
                }
                joinSetting = joinSettingView.setting;
                ChattingView chattingView = new( ref joinSetting, 1 );
                chattingView.ShowDialog();
            }
            
        }
        private void ShowAboutPage( object? _ )
        {
            // show the About page
            AboutView aboutWin = new();
            aboutWin.ShowDialog();
        }

    }
}
