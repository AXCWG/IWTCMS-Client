using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using SuperSimpleTcp;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ws
{
    public partial class MainWindow : Window
    {
        SimpleTcpClient client;

        public MainWindow()
        {
            InitializeComponent();

            command.KeyDown
                += (s, e) =>
                {
                    if (e.Key == Avalonia.Input.Key.Return)
                    {
                        Button_Click_1(null, null!);
                        command.Text = null;
                    }
                };
            output.TextChanged += (s, e) =>
            {
                scrollv.ScrollToEnd();
            };
        }


        private void authorizeButton(object sender, RoutedEventArgs e)
        {
            try
            {
                string hashedString = String.Empty;
                byte[] sha = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password.Text!));
                foreach (byte thebyte in sha)
                {
                    hashedString += thebyte.ToString("x2");
                }
                client.Send(Encoding.UTF8.GetBytes("iwtcms_login " + hashedString + "\n"));
            }
            catch (NullReferenceException ex)
            {
                output.Text += "Please connect first. \n";
            }
            catch (Exception ex)
            {
                output.Text += ex + "\n";
            }
            scrollv.ScrollToEnd();

        }

        private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {

                disconnect.IsEnabled = true;
                host.IsEnabled = false;
                port.IsEnabled = false;
                connectbutton.IsEnabled = false;
                command.IsEnabled = true;
                sendbutton.IsEnabled = true;
                client = new SimpleTcpClient(host.Text + ":" + port.Text);
                client.Connect();
                client.Events.DataReceived += (s, e) =>
                {
                    
                    Dispatcher.UIThread.Post(() =>
                    {
                        output.Text += Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count);
                    });
                };



            }
            catch (Exception ex)
            {
                disconnect.IsEnabled = false;

                host.IsEnabled = true;
                port.IsEnabled = true;
                connectbutton.IsEnabled = true;
                command.IsEnabled = false;
                sendbutton.IsEnabled = false;
                output.Text += ex + "\n";
            }

        }

        private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

            client.Send(command.Text! + "\n");


        }

        private void Button_Click_2(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            client.Disconnect();
            client = null!;
            disconnect.IsEnabled = false;
            host.IsEnabled = true;
            port.IsEnabled = true;
            connectbutton.IsEnabled = true;
            command.IsEnabled = false;
            sendbutton.IsEnabled = false;
        }
        
        private void tcpConnect(object? sender, RoutedEventArgs e)
        {
            try
            {

                disconnect.IsEnabled = true;
                host.IsEnabled = false;
                port.IsEnabled = false;
                connectbutton.IsEnabled = false;
                command.IsEnabled = true;
                sendbutton.IsEnabled = true;
                client = new SimpleTcpClient(host.Text+":"+port.Text, true, null, null);
                client.Settings.MutuallyAuthenticate = false;
                
                client.Settings.AcceptInvalidCertificates = false;
                client.Connect();
                client.Events.DataReceived += (s, e) =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        output.Text += Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count);
                       
                    });
                };



            }
            catch (Exception ex)
            {
                disconnect.IsEnabled = false;

                host.IsEnabled = true;
                port.IsEnabled = true;
                connectbutton.IsEnabled = true;
                command.IsEnabled = false;
                sendbutton.IsEnabled = false;
                output.Text += ex + "\n";
            }
        }

        private void Button_PointerEntered(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            output.Text += "Not yet done. \n";
        }
    }
}