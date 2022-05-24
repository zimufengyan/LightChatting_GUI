using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightChatting_GUI
{
    public class User : ViewModelBase
    {
        private string _username;
        public string UserName
        {
            get => _username;
            set => SetProperty( ref _username, value );
        }
    }
}
