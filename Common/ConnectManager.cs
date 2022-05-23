using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

namespace LightChatting_GUI.Common
{
    public struct AcceptHandle
    {
        public Socket server;
        public string name;
        public SendDel senDel;
        public AcceptHandle(string name, Socket socket, SendDel sendDel )
        {
            this.name = name;
            this.server = socket;
            this.senDel = sendDel;
        }
    }
    class ConnectManager
    {
        public static Dictionary<String, Socket> users = new();
        public static List<String> usernames = new();

        public static void Accept(string name, Socket server, SendDel sendDel )
        {
            AcceptHandle handle = new( name, server, sendDel );
            try
            {
                server.BeginAccept( new AsyncCallback( AcceptCallback ), handle );

            }
            catch ( Exception e )
            {
                Console.WriteLine( e.Message );
            }
        }

        private static void AcceptCallback( IAsyncResult ar )
        {
            try
            {
                // 获取客户端套接字
                AcceptHandle handle = (AcceptHandle)ar.AsyncState;
                Socket client = handle.server.EndAccept( ar );
                Console.WriteLine( string.Format( "[INFO] Succeed to build connection with Client {0}", client.RemoteEndPoint ) );
                // 接收Client的个人信息
                string name = RSManager.Receive( client );
                name = name.Trim();
                users.Add( name, client );
                usernames.Add( name );
                // 发送自身的个人信息
                RSManager.Send( client, handle.name );
                Debug.WriteLine( string.Format( "[INFO] New Cinet name: {0}", name ) );
                // 保持对Client的信息接收
                RSManager.ReceiveAsync( client, name, handle.senDel );
                // 继续回应下一个连接请求
                handle.server.BeginAccept( new AsyncCallback( AcceptCallback ), handle );
            }
            catch (Exception e )
            {
                Debug.WriteLine( e.Message );
            }
        }

        public static void Delete(string name )
        {
            // 当某个Client连接断开时，从已有连接中删除
            usernames.Remove( name );
            users.Remove( name );
        }
    }
}
