using System;
using System.IO;
using System.Net.Http;
using System.Net.WebSockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using SuperSimpleTcp;

namespace ws;

public partial class MainView : UserControl
{
    public static ClientWebSocket? socket = new ClientWebSocket();
    public static HttpClient http = new HttpClient();
    private Window parentWindow;
    private Config config; 
        public MainView(Window parentWindow, Config config)
        {
            this.config = config;
            this.parentWindow = parentWindow;
            InitializeComponent();
            host.Text = config.Host;
            password.Text = config.Password;
            port.Value = config.Port;
            

            command.KeyDown
                += (s, e) =>
                {
                    if (e.Key == Avalonia.Input.Key.Return)
                    {
                        Send(null, null!);
                        command.Text = null;
                    }
                };
            // password.KeyDown += (s, e) =>
            // {
            //     if (e.Key == Avalonia.Input.Key.Return)
            //     {
            //         authorizeButton(null!, null!);
            //         password.Text = null;
            //     }
            // };
            output.TextChanged += (s, e) =>
            {
                scrollv.ScrollToEnd();
            };
        }


        

        private void Connect(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            
        }

        private void Send(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }

        private void Disconnect(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            
        }

        private async void TlsConnect(object? sender, RoutedEventArgs e)
        {
           
        }

        private void ChangeWrap(object? sender, RoutedEventArgs e)
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


        private void Host_OnTextChanged(object? sender, TextChangedEventArgs e)
        {
            config.Host = (sender as TextBox)!.Text!; 
        }
}