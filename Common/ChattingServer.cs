using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Diagnostics;

namespace LightChatting_GUI.Common
{
    public class ChattingServer
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public string Name { get; set; }
        public bool Connected
        {
            get
            {
                return this.socket.Connected;
            }
        }
        private Socket socket;

        public ChattingServer(string ip, int port, string name )
        {
            this.IP = ip;
            this.Port = port;
            this.Name = name.Trim();
            // 创建套接字
            IPEndPoint ipe = new( IPAddress.Parse( this.IP ), this.Port );
            socket = new( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            socket.Bind( ipe );
            Debug.WriteLine( "[INFO] Server had been Initialized, watting connectting..." );
        }

        public void Listen()
        {
            socket.Listen( 100 );
            ConnectManager.Accept( this.Name, this.socket, this.Send );
            
        }

        public void Send( string buffer, string sourceName=null, 
            List<string>ignoreClinets=null, string targetName=null, int index=-1)
        {
            string sendBuffer;
            if ( sourceName == null )
            {
                sendBuffer = this.Name + "#" + buffer;
            }
            else
            {
                sendBuffer = sourceName +  "#" + buffer;
            }
            if (index != -1 )
            {
                SendTo( index, sendBuffer );
            }
            else if (targetName != null )
            {
                SendTo( targetName, sendBuffer );
            }
            else 
            {
                foreach ( string n in ConnectManager.usernames )
                {
                    if ( ignoreClinets != null && ignoreClinets.Contains( n ) )
                    {
                        continue;
                    }
                    SendTo( n, sendBuffer );
                }
            }
        }

        private static void SendTo(string name, string buffer )
        {
            Socket client;
            if (ConnectManager.users.TryGetValue( name, out client ) )
            {
                RSManager.SendAsync( name, client, buffer );
            }
            else
            {
                Debug.WriteLine( string.Format( "[IRROR] this client of {0} is not existing.", name ) );
            }
        }
        private static void SendTo(int index, string buffer )
        {
            if(ConnectManager.usernames.Count > index )
            {
                SendTo( ConnectManager.usernames[index], buffer );
            }
            else
            {
                Debug.WriteLine( string.Format( "[IRROR] this {0}th client is not existing.", index ) );
            }
        }

        ~ChattingServer()
        {
            this.socket.Close();
        }

        public void Close()
        {
            try
            {
                if ( this.socket != null && this.socket.Connected )
                {
                    this.socket.Shutdown( SocketShutdown.Both );
                }
            }
            catch (Exception e )
            {
                Debug.WriteLine( e.Message );
            }
            this.socket.Close();
        }
 
    }
}
