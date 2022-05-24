﻿using System;
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
        private User user;
        public string Name { get => user.UserName; }
        public bool Connected { get
            {
                return this.socket.Connected;
            } 
        }

        private AddUserDel addDel;
        private IPEndPoint serverIpe;
        private Socket socket;

        private ManualResetEvent connectDone = new( false );
        public ChattingClient( string ip, int port, 
            ref User user, AddUserDel addDel )
        {
            this.remoteIP = ip;
            this.remotePort = port;
            this.user = user;
            this.addDel = addDel;
            this.serverIpe = new( IPAddress.Parse( this.remoteIP ), this.remotePort );
            this.socket = new( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
        }
        public void Connect()
        {
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
