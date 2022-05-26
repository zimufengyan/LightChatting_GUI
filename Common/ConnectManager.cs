using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using LightChatting_GUI;
using System.ComponentModel;

namespace LightChatting_GUI.Common
{
    public struct AcceptHandle
    {
        public readonly Socket server;
        public readonly string userName;
        public SendDel senDel;
        public AddUserDel addDel;
        public AcceptHandle(string userName, Socket socket, 
            SendDel sendDel, AddUserDel addDel )
        {
            this.userName = userName;
            this.server = socket;
            this.senDel = sendDel;
            this.addDel = addDel;
        }
    }
    class ConnectManager
    {
        public static Dictionary<String, Socket> users = new();
        public static List<string> userNames = new();

        public static void Accept( string userName, Socket server,
            SendDel sendDel, AddUserDel addDel )
        {
            AcceptHandle handle = new( userName, server, sendDel, addDel );
            try
            {
                server.BeginAccept( new AsyncCallback( AcceptCallback ), handle );

            }
            catch ( Exception e )
            {
                Debug.WriteLine( e.Message );
            }
        }

        private static void AcceptCallback( IAsyncResult ar )
        {
            try
            {
                // 获取客户端套接字
                AcceptHandle handle = (AcceptHandle)ar.AsyncState;
                Socket client = handle.server.EndAccept( ar );
                Debug.WriteLine( string.Format( "[INFO] Succeed to build connection with Client {0}", client.RemoteEndPoint ) );
                // 接收Client的个人信息
                string name = RSManager.Receive( client );
                name = name.Trim();
                if ( name == handle.userName )
                {
                    name += "1";
                }
                // handle.addDel( name );
                AddUser( name, client );
                // 发送自身的个人信息
                RSManager.Send( client, handle.userName );
                Debug.WriteLine( string.Format( "[INFO] New Cinet name: {0}", name ) );
                // 保持对Client的信息接收
                RSManager.ReceiveAsync( client, name, handle.senDel );
                // 继续回应下一个连接请求
                handle.server.BeginAccept( new AsyncCallback( AcceptCallback ), handle );
            }
            catch ( Exception e )
            {
                Debug.WriteLine( e.Message );
            }
        }

        public static void Delete(string name )
        {
            // 当某个Client连接断开时，从已有连接中删除
            // usernames.Remove( name );
            users.Remove( name );
        }
        public static void AddUser( string name, Socket socket = null )
        {
            if ( !userNames.Contains( name ) )
            {
                userNames.Add( name );
                if (socket != null )
                {
                    users.Add( name, socket );
                }
            }
        }
        public static void Clear()
        {
            userNames.Clear();
            users.Clear();
        }

        public static int Count()
        {
            return userNames.Count;
        }
    }
    
}
