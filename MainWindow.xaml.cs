using System.Windows;
using System.Windows.Input;

namespace LightChatting_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // 窗口拖动
            this.Header.MouseLeftButtonDown += ( o, e ) => { DragMove(); };
            this.DataContext = new MainViewModel();
        }

        private void MinimizeWindow_Exec( object sender, ExecutedRoutedEventArgs e )
        {
            SystemCommands.MinimizeWindow( this );

        }
        private void CloseWindow_Exec( object sender, ExecutedRoutedEventArgs e )
        {
            SystemCommands.CloseWindow( this );
        }
        

    }
}
