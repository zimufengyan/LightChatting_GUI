using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Threading;

namespace LightChatting_GUI
{
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

        public string Message 
        { 
            get => message; 
            set => SetProperty( ref message, value );
        }
        private string roomInfo;
        public string RoomInfo
        {
            get => roomInfo;
            set => SetProperty( ref roomInfo, value );
        }

        public ChattingViewModel( string ip, int port, string name, int mode = 0 )
        {
            Mode = mode;
            if ( mode == 0 )
            {   // 创建Server
                Server = new Common.ChattingServer( ip, port, name );
                Server.Listen();
            }
            else
            {  // 创建Client
                Client = new Common.ChattingClient( ip, port, name );
                Client.Connect();
            }
            RoomInfo = ip + ":" + port.ToString();
            
        }

        private void SendMessage(object? _ )
        {
            if (Mode == 0 )
            {
                Server.Send( Message );
            }
            else
            {
                Client.Send( Message );
            }
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