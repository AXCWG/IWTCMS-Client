using System;
using System.Net.Http;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Websocket.Client;

namespace ws;

public partial class MainView : UserControl
{
    public static WebsocketClient? socket;
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
        output.TextChanged += (s, e) => { scrollv.ScrollToEnd(); };
    }


    private async void Connect(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        disconnect.IsEnabled = true;
        host.IsEnabled = false;
        port.IsEnabled = false;
        connectbutton.IsEnabled = false;
        connecttls.IsEnabled = false;
        command.IsEnabled = true;
        sendbutton.IsEnabled = true;

        // http.CancelPendingRequests();
        // More HTTP bullshit. 
        // Yet to be waited for the new fix for any user login. 


        socket = new WebsocketClient(new Uri($"ws://{host.Text}:{port.Value}/ws/console"));
        socket!.MessageReceived.Subscribe(msg =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                output.Text += msg.Text == "Connected to iwtcms logs" ? msg.Text + "\n" : msg.Text;
            });
        });
        await socket.Start();
    }

    private void Send(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        socket!.Send(command.Text!);
    }

    private void Disconnect(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        socket!.Dispose();
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