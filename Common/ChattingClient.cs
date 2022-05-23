using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace LightChatting_GUI.Common
{
    public class ChattingClient
    {
        public string remoteIP { get; set; }
        public int remotePort { get; set; }
        public string Name { get; set; }
        public bool Connected { get
            {
                return this.socket.Connected;
            } 
        }

        private IPEndPoint serverIpe;
        private Socket socket;
        private List<string> remoteName = new();    // 存放其他远端主机用户的昵称，首项为Server昵称。

        private ManualResetEvent connectDone = new( false );
        public ChattingClient( string ip, int port, string name )
        {
            this.remoteIP = ip;
            this.remotePort = port;
            this.Name = name;
        }
        public ChattingClient()
        {

        }
        public void Connect()
        {
            this.serverIpe = new( IPAddress.Parse( this.remoteIP ), this.remotePort );
            this.socket = new( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            this.socket.BeginConnect( this.serverIpe, asyncResult =>
            {
                if ( !this.socket.Connected )
                {
                    Debug.WriteLine( "[ERROR] 服务器未开启，尝试重连..." );
                    this.Connect();
                }
                else
                {
                    this.socket.EndConnect( asyncResult );
                    Debug.WriteLine( "[INFO] Succeed to connect the remote host..." );
                    // 发送个人信息给Server
                    RSManager.Send( this.socket, this.Name );
                    // 接收Server的个人信息
                    string serverName = RSManager.Receive( this.socket );
                    this.remoteName.Add( serverName );
                    connectDone.Set();
                    // 保持对Server的信息接收
                    RSManager.ReceiveAsync( this.socket, serverName );
                }
            }, null );
            connectDone.WaitOne();

            return;
        }

        public void Send( string buffer )
        {
            string newBuffer = Name + "#" + buffer;
            RSManager.SendAsync( Name, socket, newBuffer );
        }

        public void Close()
        {
            this.socket.Close();
        }
    }

}
