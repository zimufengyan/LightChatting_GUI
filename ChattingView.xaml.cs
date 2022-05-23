using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Windows.Controls;
using System;
using System.Windows.Threading;
using System.ComponentModel;

namespace LightChatting_GUI
{
    /// <summary>
    /// ChattingView.xaml 的交互逻辑
    /// </summary>
    public partial class ChattingView : Window
    {
        private BackgroundWorker bgWorker = new();
        public ChattingView()
        {
            InitializeComponent();
            this.Header.MouseLeftButtonDown += ( o, e ) => { DragMove(); };
            // 开启一个线程负责Message的抓取与展示
            bgWorker.DoWork += ShowMessage;
            bgWorker.RunWorkerAsync();
        }

        private void Header_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            DragMove();
        }
        public void ShowMessage( object sender, DoWorkEventArgs e )
        {
            StackPanel stackPanel = this.MessagePanel;
            while ( true )
            {
                if ( Common.RSManager.messageQuene.Count > 0 )
                {
                    string buffer = Common.RSManager.messageQuene.Dequeue();
                    Common.ReceivedMessagehandle messageHandle =
                            Common.MessageManager.ReceivedMessageProcessor( buffer );
                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            MessageView messageView = new MessageView();
                            messageView.Name.Content = messageHandle.name;
                            messageView.Message.Text = messageHandle.buffer;
                            stackPanel.Children.Add( messageView );
                        } );

                }
            }
        }
    }
}
