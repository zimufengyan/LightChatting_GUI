using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Windows.Controls;
using System;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Media;

namespace LightChatting_GUI
{
    /// <summary>
    /// ChattingView.xaml 的交互逻辑
    /// </summary>
    public partial class ChattingView : Window
    {
        private BackgroundWorker bgWorker = new();

        public ChattingView( ref SettingDialogViewModel setting, int mode = 0 )
        {
            InitializeComponent();
            // 窗口拖动
            this.Header.MouseLeftButtonDown += ( o, e ) => { DragMove(); };
            // 创建VieweModel
            ChattingViewModel chatting = new( ref setting, mode );
            this.DataContext = chatting;
            this.UsersList.ItemsSource = chatting.UsersList;
            // 开启一个线程负责Message的抓取与展示
            bgWorker.DoWork += ShowMessage;
            bgWorker.RunWorkerAsync();
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
                            MessageView messageView = new();
                            messageView.Name.Content = messageHandle.name;
                            messageView.Message.Text = messageHandle.buffer;
                            /*
                            if ( Chatting.Name.ToString().Trim() == messageHandle.name.Trim() )
                            {
                                messageView.Message.Background = new SolidColorBrush(
                                    Color.FromRgb( 177, 188, 230 ) );
                            }
                            */
                            stackPanel.Children.Add( messageView );
                        } );

                }
            }
        }
    }
}
