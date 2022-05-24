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

        private SettingDialogViewModel createSetting;
        private SettingDialogViewModel joinSetting;
        private SettingDialogView createSettingView = new();
        private SettingDialogView joinSettingView = new();
        private async void ExecuteRunCreateDialog( object? _ )
        {
            //show the dialog
            var result = await DialogHost.Show( createSettingView, "SettingDialog", ClosingEventHandler );
            if ( result.ToString() == "Accept" )
            {
                createSetting = createSettingView.setting;
                Name = createSettingView.setting.DefaultName;
            }
        }
        private async void ExecuteRunJoinDialog( object? _ )
        {
            //show the dialog
            var result = await DialogHost.Show( joinSettingView, "SettingDialog", ClosingEventHandler );
            if ( result.ToString() == "Accept" )
            {
                joinSetting = joinSettingView.setting;
            }
        }
        private void ClosingEventHandler( object sender, DialogClosingEventArgs eventArgs )
        {
            
        }
        private void CreateChatting( object? _ )
        {
            if ( createSetting == null || !createSetting.CanChatting )
            {
                ExecuteRunCreateDialog( null );
                return;
            }
            ChattingView chattingView = new( ref createSetting, 0 );
            chattingView.ShowDialog();
        }

        private void JoinChatting( object? _ )
        {
            if ( joinSetting == null || !joinSetting.CanChatting )
            {
                ExecuteRunJoinDialog( null );
                return;
            }
            ChattingView chattingView = new( ref joinSetting, 1 );
            chattingView.ShowDialog();
        }

    }
}
