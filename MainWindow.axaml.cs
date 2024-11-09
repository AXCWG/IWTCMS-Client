using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using SuperSimpleTcp;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace ws
{
    public partial class MainWindow : Window
    {
        public static SimpleTcpClient? client;

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
            password.KeyDown += (s, e) =>
            {
                if (e.Key == Avalonia.Input.Key.Return)
                {
                    authorizeButton(null!, null!);
                    password.Text = null;
                }
            };
            output.TextChanged += (s, e) =>
            {
                scrollv.ScrollToEnd();
                // output.CaretIndex = int.MaxValue;
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
                client!.Send(Encoding.UTF8.GetBytes("iwtcms_login " + hashedString + "\n"));
            }
            catch (NullReferenceException ex)
            {
                output.Text += $"{ex.Message}\n";
                output.Text += "Please connect first. \n";
            }
            catch (Exception ex)
            {
                output.Text += ex + "\n";
            }
            scrollv.ScrollToEnd();
            // output.CaretIndex = int.MaxValue;

        }

        private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {

                disconnect.IsEnabled = true;
                host.IsEnabled = false;
                port.IsEnabled = false;
                connectbutton.IsEnabled = false;
                connecttls.IsEnabled = false;

                command.IsEnabled = true;
                sendbutton.IsEnabled = true;
                client = new SimpleTcpClient(host.Text + ":" + port.Text);
                client.Connect();
                client.Events.DataReceived += (s, e) =>
                {

                    Dispatcher.UIThread.Post(() =>
                    {
                        output.Text += Encoding.UTF8.GetString(e.Data.Array!, 0, e.Data.Count);
                    });
                };
                output.Text += "Connection successful\n";



            }
            catch (Exception ex)
            {
                disconnect.IsEnabled = false;
                connecttls.IsEnabled = true;

                host.IsEnabled = true;
                port.IsEnabled = true;
                connectbutton.IsEnabled = true;
                command.IsEnabled = false;
                sendbutton.IsEnabled = false;
                output.Text += $"Unsuccessful: {ex.Message}\nFull trace: \n{ex} \n";
            }

        }

        private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

            client!.Send(command.Text! + "\n");


        }

        private void Button_Click_2(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            client!.Disconnect();
            client = null!;
            disconnect.IsEnabled = false;
            host.IsEnabled = true;
            port.IsEnabled = true;
            connectbutton.IsEnabled = true;
            connecttls.IsEnabled = true;

            command.IsEnabled = false;
            sendbutton.IsEnabled = false;
            pfxReturn.disableCertCheck = false;
            pfxReturn.hasEntry = false;
            output.Text += ("Disconnected. \n");

        }

        private async void tlsConnect(object? sender, RoutedEventArgs e)
        {
            var dia = new tlsWindow();
            await dia.ShowDialog(this);

            if (pfxReturn.hasEntry && !pfxReturn.disableCertCheck)
            {
                try
                {
                    disconnect.IsEnabled = true;
                    host.IsEnabled = false;
                    port.IsEnabled = false;
                    connectbutton.IsEnabled = false;
                    connecttls.IsEnabled = false;
                    command.IsEnabled = true;
                    sendbutton.IsEnabled = true;
                    client = new SimpleTcpClient(host.Text + ":" + port.Text, true,
                    pfxReturn.Dir
                    ,
                    pfxReturn.Password
                    );
                    client.Settings.MutuallyAuthenticate = false;

                    client.Settings.AcceptInvalidCertificates = false;
                    client.Connect();
                    client.Events.DataReceived += (s, e) =>
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            output.Text += Encoding.UTF8.GetString(e.Data.Array!, 0, e.Data.Count);

                        });
                    };
                    output.Text += "Connection successful\n";
                }
                catch (Exception ex)
                {
                    disconnect.IsEnabled = false;
                    connecttls.IsEnabled = true;
                    host.IsEnabled = true;
                    port.IsEnabled = true;
                    connectbutton.IsEnabled = true;
                    command.IsEnabled = false;
                    sendbutton.IsEnabled = false;
                    output.Text += $"Unsuccessful: {ex.Message}\nFull trace: \n{ex}\n";
                }

            }
            if (pfxReturn.disableCertCheck && pfxReturn.hasEntry)
            {
                try
                {
                    var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ws.cert.pfx");
                    Directory.CreateDirectory("cert");
                    using (FileStream file = File.Create("cert/cert.pfx"))
                    {
                        stream.CopyTo(file);
                    }


                    disconnect.IsEnabled = true;
                    host.IsEnabled = false;
                    port.IsEnabled = false;
                    connectbutton.IsEnabled = false;
                    connecttls.IsEnabled = false;
                    command.IsEnabled = true;
                    sendbutton.IsEnabled = true;
                    client = new SimpleTcpClient(host.Text + ":" + port.Text, true,
                    "cert/cert.pfx", "0000"
                    );
                    client.Settings.MutuallyAuthenticate = false;

                    client.Settings.AcceptInvalidCertificates = true;
                    client.Connect();
                    client.Events.DataReceived += (s, e) =>
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            output.Text += Encoding.UTF8.GetString(e.Data.Array!, 0, e.Data.Count);

                        });
                    };
                    output.Text += "Connection successful\n";

                }
                catch (Exception ex)
                {
                    disconnect.IsEnabled = false;
                    connecttls.IsEnabled = true;
                    host.IsEnabled = true;
                    port.IsEnabled = true;
                    connectbutton.IsEnabled = true;
                    command.IsEnabled = false;
                    sendbutton.IsEnabled = false;
                    output.Text += $"Unsuccessful: {ex.Message}\nFull trace: \n{ex}\n";
                }


            }
        }
        private void changeWrap(object? sender, RoutedEventArgs e)
        {

            if (checkbox.IsChecked!.Value)
            {
                output.TextWrapping = Avalonia.Media.TextWrapping.Wrap;

            }
            if (!checkbox.IsChecked.Value)
            {
                output.TextWrapping = Avalonia.Media.TextWrapping.NoWrap;
            }
        }
    }


}
