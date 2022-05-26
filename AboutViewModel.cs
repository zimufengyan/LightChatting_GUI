using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LightChatting_GUI
{
    public class AboutViewModel
    {
        public CommandBase CloseCommand => new( CloseAbout );

        public void CloseAbout( object? obj )
        {
            var win = obj as Window;
            win.Close();
        }
    }
}
