using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightChatting_GUI.Common
{
    public struct ReceivedMessagehandle
    {
        public string name;        // 发送者的昵称
        public string buffer;      // 实际发送的内容
    }
    public class MessageManager
    {
        public static ReceivedMessagehandle ReceivedMessageProcessor(string buffer)
        {
            ReceivedMessagehandle handle = new();
            try
            {
                string[] result;
                char[] separator = new char[] { '#' };
                result = buffer.Split( separator, StringSplitOptions.None );
                handle.name = result[0].Trim();
                handle.buffer = result[1];
            }
            catch(Exception e )
            {
                Debug.WriteLine( "[ERROR in message] " + e.Message );
            }
            return handle;
        }


    }
}
