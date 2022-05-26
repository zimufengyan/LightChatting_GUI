using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LightChatting_GUI
{
    /// <summary>
    /// AboutView.xaml 的交互逻辑
    /// </summary>
    public partial class AboutView : Window
    {
        
        public AboutView()
        {
            InitializeComponent();
            // 窗口拖动
            this.Header.MouseLeftButtonDown += ( o, e ) => { DragMove(); };
        }

        private void CloseWindow_Exec( object sender, ExecutedRoutedEventArgs e )
        {
            SystemCommands.CloseWindow( this );
        }


    }
}
