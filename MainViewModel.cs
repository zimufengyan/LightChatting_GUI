using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using System.Threading.Tasks;
using System.Windows;

namespace LightChatting_GUI
{
    public class MainViewModel : ViewModelBase
    {
        public CommandBase CreateChatingCommand => new( CreateChatting );
        public CommandBase JoinChattingCommand => new( JoinChatting );

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty( ref _name, value );
        }

        private SettingDialogViewModel createSettting;
        private SettingDialogViewModel joinSettting;
        private SettingDialogView createSettingView = new()
        {
            DataContext = new SettingDialogViewModel()
        };
        private SettingDialogView joinSettingView = new()
        {
            DataContext = new SettingDialogViewModel()
        };

        private async void ExecuteRunCreateDialog( object? _ )
        {
            //show the dialog
            var result = await DialogHost.Show( createSettingView, "SettingDialog", ClosingEventHandler );
            if ( result.ToString() == "Accept" )
            {
                createSettting = (SettingDialogViewModel)createSettingView.DataContext;
                Name = createSettting.DefaultName;
            }
        }
        private async void ExecuteRunJoinDialog( object? _ )
        {
            //show the dialog
            var result = await DialogHost.Show( joinSettingView, "SettingDialog", ClosingEventHandler );
            if ( result.ToString() == "Accept" )
            {
                joinSettting = (SettingDialogViewModel)joinSettingView.DataContext;
            }
        }
        private void ClosingEventHandler( object sender, DialogClosingEventArgs eventArgs )
        {
            
        }
        private void CreateChatting( object? _ )
        {
            if ( createSettting == null || !createSettting.CanChatting )
            {
                ExecuteRunCreateDialog( null );
                return;
            }
            ChattingViewModel chatting = new( 
                createSettting.IpAddress, createSettting.TruePort, Name );
            ChattingView chattingView = new()
            {
                DataContext = chatting
            };
            chattingView.ShowDialog();
        }

        private void JoinChatting( object? _ )
        {
            if ( joinSettting == null || !joinSettting.CanChatting )
            {
                ExecuteRunJoinDialog( null );
                return;
            }
            ChattingViewModel chatting = new( 
                joinSettting.IpAddress, joinSettting.TruePort, 
                joinSettting.DefaultName, 1 );
            ChattingView chattingView = new()
            {
                DataContext = chatting
            };
            chattingView.ShowDialog();
        }

    }
}
