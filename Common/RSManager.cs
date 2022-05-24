using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace LightChatting_GUI.Common
{
    public delegate void SendDel( string buffer, string sourceName = null, 
        List<string> ignoreClinets = null, string name = null, int index = -1 );

    public struct RecieveHandle
    {
        public SendDel sendDel;
        public string name;
        public Socket socket;
        public RecieveHandle(string name, Socket socket, SendDel sendDel )
        {
            this.name = name;
            this.socket = socket;
            this.sendDel = sendDel;
        }
    }
    public class RSManager
    {
        static byte[] receivedData = new byte[1024];
        public static Queue<string> messageQuene = new();
        public static void SendAsync( string name, Socket socket, string buffer )
        {
            if ( socket == null || buffer == string.Empty )
            {
                return;
            }
            byte[] sendData = Encoding.UTF8.GetBytes( buffer );
            try
            {
                // 开始发送消息
                socket.BeginSend( sendData, 0, sendData.Length, SocketFlags.None,
                    asyncResult =>
                    {
                        int length = socket.EndSend( asyncResult );
                        if (length > 0 )
                        {
                            messageQuene.Enqueue( buffer );
                        }
                    }, null );
            }
            catch ( SocketException )
            {
                // 删除该连接
                ConnectManager.Delete( name );
                Debug.WriteLine( string.Format( "{0} had exited the room.", name ) );
                return;
            }
        }
        public static void Send( Socket socket, string buffer )
        {
            if ( socket == null || buffer == string.Empty )
            {
                return;
            }
            byte[] sendData = Encoding.UTF8.GetBytes( buffer );
            try
            {
                socket.Send( sendData, sendData.Length, SocketFlags.None );
            }
            catch ( Exception e )
            {
                Debug.WriteLine( e.Message );
            }
        }
        public static void SendToAsync( Socket source, Socket target, string buffer )
        {
            if ( source == null || target == null || buffer == string.Empty || !target.Connected )
            {
                return;
            }
            byte[] sendData = Encoding.UTF8.GetBytes( buffer );
            try
            {
                // 开始发送消息
                source.BeginSendTo( sendData, 0, sendData.Length,
                    SocketFlags.None, target.RemoteEndPoint,
                    asyncResult =>
                    {
                        int length = source.EndSendTo( asyncResult );
                    }, null );
            }
            catch ( Exception e )
            {
                Debug.WriteLine( e.Message );
            }
        }

        public static string Receive( Socket socket )
        {
            if ( socket == null )
            {
                return string.Empty;
            }
            try
            {
                // 开始同步接收信息。
                Array.Clear( receivedData, 0, receivedData.Length );
                int length = socket.Receive( 
                    receivedData, receivedData.Length, SocketFlags.None );
                // 根据实际长度对receivedData做位截断（拷贝到新数组）
                // 不做截断则buffer长度始终为receivedData长度
                byte[] tempData = new byte[length];
                Array.Copy( receivedData, tempData, length );
                string buffer = Encoding.UTF8.GetString( tempData );
                return buffer;
            }
            catch ( Exception e )
            {
                Debug.WriteLine( e.Message );
                return string.Empty;
            }

        }
        public static void ReceiveAsync( Socket socket, string name,
            SendDel sendDel = null )
        {
            RecieveHandle handle = new( name, socket, sendDel );
            try
            {
                Array.Clear( receivedData, 0, receivedData.Length );
                socket.BeginReceive( receivedData, 0, receivedData.Length, SocketFlags.None,
                    new AsyncCallback( RSManager.ReceiveCallback ), handle );
            }
            catch ( SocketException e )
            {
                Debug.WriteLine( e.Message );
            }
        }
        private static void ReceiveCallback( IAsyncResult ar )
        {
            RecieveHandle handle = (RecieveHandle)ar.AsyncState;
            try
            {
                int length = handle.socket.EndReceive( ar );
                if ( length > 0 )
                {
                    // 根据实际长度对receivedData做位截断（拷贝到新数组）
                    byte[] tempData = new byte[length];
                    Array.Copy( receivedData, tempData, length );
                    string buffer = Encoding.UTF8.GetString( tempData );
                    ReceivedMessagehandle messageHandle =
                            MessageManager.ReceivedMessageProcessor( buffer );
                    if ( messageHandle.buffer != string.Empty )
                    {
                        Debug.WriteLine( string.Format(
                            "{0}: {1}", messageHandle.name, messageHandle.buffer ) );
                        messageQuene.Enqueue(buffer);
                        if ( handle.sendDel != null )
                        {   // 如果为Server端，则将消息转发给其余Clients
                            List<string> ignoreClients = new();
                            ignoreClients.Add( messageHandle.name );
                            handle.sendDel( messageHandle.buffer,
                                messageHandle.name, ignoreClients );
                        }
                    }
                    
                }
                Array.Clear( receivedData, 0, receivedData.Length );
                handle.socket.BeginReceive( receivedData, 0, receivedData.Length, SocketFlags.None,
                    new AsyncCallback( RSManager.ReceiveCallback ), handle );

            }
            catch ( SocketException )
            {
                // 删除该连接
                ConnectManager.Delete( handle.name );
                Debug.WriteLine( string.Format( "{0} had exited the room.", handle.name ) );
                return;
            }
        }
    }

}
