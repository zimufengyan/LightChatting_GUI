/*
 * ChattingView的ViewModel文件
 * 完成聊天窗口的命令绑定、数据交互，实现与用户的交互逻辑
 */

using System.Windows.Controls;
using System.ComponentModel;

namespace LightChatting_GUI
{
    public delegate void AddUserDel( string name );
    public class ChattingViewModel : ViewModelBase
    {
        public CommandBase SendMessageCommand => new( SendMessage );
        public CommandBase CloseCommand => new( CloseChatting );

        private Common.ChattingServer _server;
        public Common.ChattingServer Server
        {
            get => _server;
            set => SetProperty( ref _server, value );
        }

        private Common.ChattingClient _client;
        public Common.ChattingClient Client
        {
            get => _client;
            set => SetProperty( ref _client, value );
        }

        private int mode;
        public int Mode
        {
            get => mode;
            set => SetProperty( ref mode, value );
        }
        private string message;
        public string Message { get => message; set => SetProperty( ref message, value ); }

        private string roomInfo;
        public string RoomInfo
        {
            get => roomInfo;
            set => SetProperty( ref roomInfo, value );
        }

        public BindingList<User> UsersList = new();

        public ChattingViewModel( ref SettingDialogViewModel setting, int mode = 0 )
        {
            Mode = mode;
            User user = new() { UserName = setting.DefaultName };
            if ( mode == 0 )
            {   // 创建Server
                Server = new Common.ChattingServer( 
                    setting.IpAddress, setting.TruePort, 
                    ref user, this.AddUser );
                this.UsersList.Add( user );
                Server.Listen();
            }
            else
            {  // 创建Client
                Client = new Common.ChattingClient(
                    setting.IpAddress, setting.TruePort, 
                    ref user, this.AddUser );
                Client.Connect();
            }
            RoomInfo = setting.IpAddress + ":" + setting.Port;
            
        }

        private void SendMessage( object obj )
        {
            TextBox box = obj as TextBox;
            if ( Mode == 0 )
            {
                Server.Send( Message );
            }
            else
            {
                Client.Send( Message );
            }
            box.Clear();
        }

        public void AddUser( string name )
        {
            foreach ( User user in UsersList )
            {
                if ( user.UserName == name )
                {
                    return;
                }
            }
            UsersList.Add( new User() { UserName = name } );
        }
        public void CloseChatting( object? obj )
        {
            if ( Mode == 0 )
            {
                Server.Close();
            }
            else
            {
                Client.Close();
            }
            var win = obj as System.Windows.Window;
            win.Close();
        }

        

    }
}