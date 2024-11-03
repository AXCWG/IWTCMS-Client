using Avalonia.Controls;
using Avalonia.Logging;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ws
{
    public partial class MainWindow : Window
    {
        Socket? socket;
        

        public MainWindow()
        {
            InitializeComponent();
            
            command.KeyDown
                += (s, e) => {
                    if (e.Key == Avalonia.Input.Key.Return) {
                        Button_Click_1(null, null!);
                        command.Text = null;
                    }
                };
        }
        async void listen()
        {
            byte[] responseBytes = new byte[1024];
            while (true)
            {
                try
                {
                    int bytes = await socket!.ReceiveAsync(responseBytes);
                    output.Text += Encoding.UTF8.GetString(responseBytes, 0, bytes);
                    scrollv.ScrollToEnd();
                }catch(Exception ex)
                {
                    output.Text += ex;
                    break;
                }
                
            }

        }
        
        private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                disconnect.IsEnabled = true;
                host.IsEnabled = false;
                port.IsEnabled = false;
                password.IsEnabled = false;
                connectbutton.IsEnabled = false;
                command.IsEnabled = true;
                sendbutton.IsEnabled = true;
                socket = new Socket(IPEndPoint.Parse(host.Text!).AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync(IPAddress.Parse(host.Text!), ((int)port.Value!.Value));
                
                listen();

            }
            catch (Exception ex) {
                disconnect.IsEnabled = false;

                host.IsEnabled = true;
                port.IsEnabled = true;
                password.IsEnabled = true;
                connectbutton.IsEnabled = true;
                command.IsEnabled = false;
                sendbutton.IsEnabled = false;
                output.Text += ex + "\n";
            }
            
        }

        private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

            socket!.Send(Encoding.UTF8.GetBytes(command.Text! + "\n"));
            

        }

        private void Button_Click_2(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            socket!.Shutdown(SocketShutdown.Both);
            socket.Close();
            socket.Dispose();
            disconnect.IsEnabled = false;
            host.IsEnabled = true;
            port.IsEnabled = true;
            password.IsEnabled = true;
            connectbutton.IsEnabled = true;
            command.IsEnabled = false;
            sendbutton.IsEnabled = false;
        }
    }
}