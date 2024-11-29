using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Websocket.Client;

namespace ws;

public partial class MainView : UserControl
{
    public static CookieContainer cookies = new CookieContainer();

    public static HttpClientHandler httpClientHandler = new HttpClientHandler
    {
        CookieContainer = cookies,
    };

    public static WebsocketClient? socket;
    public static HttpClient http;

    private Window parentWindow;
    private Config config;

    public MainView(Window parentWindow, Config config)
    {
        
        // Some random-ass init. 
        this.config = config;
        this.parentWindow = parentWindow;
        InitializeComponent();
        
        // Init ui from config
        host.Text = config.Host;
        password.Text = config.Password;
        port.Value = config.Port;
        Username.Text = config.Username;
        password.Text = config.Password;

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

        http = new HttpClient(httpClientHandler);
        http.BaseAddress = new Uri($"http://{host.Text}:{port.Value}");
        HttpResponseMessage httpResponseMessage = await http.PostAsync("api/login",
            new StringContent(JsonSerializer.Serialize(new
            {
                username = Username.Text,
                password = password.Text,
            }), Encoding.UTF8, "application/json"));
        
        try
        {
            var headers = httpResponseMessage.Headers.GetValues("Set-Cookie").First().Split("; ");
            foreach (var header in headers)
            {
                if (header.Contains("="))
                    cookies.Add(new Cookie(header.Split("=")[0].Replace("$", ""), header.Split("=")[1].Replace(",", ""),
                        path: "/",
                        domain: host.Text));
                else cookies.Add(new Cookie(header, null, path: "/", domain: host.Text));
                ;
            }
        }
        catch (Exception ex)
        {
            output.Text += "Error: " + ex.Message +
                           "\nPotentially due to: Wrong username or password. \nHttpResponse: " +
                           $"{httpResponseMessage}\n{httpResponseMessage.Content.ReadAsStringAsync().Result}";
        }

        var factory = new Func<ClientWebSocket>(() => new ClientWebSocket
        {
            Options =
            {
                Cookies = cookies
            }
        });
        socket = new WebsocketClient(new Uri($"ws://{host.Text}:{port.Value}/ws/console"), factory);
        socket.MessageReceived.Subscribe(msg =>
        {
            Console.WriteLine(msg.Text);
            Dispatcher.UIThread.Post(() =>
            {
                if (msg.Text == "Connected to iwtcms logs")
                {
                    output.Text += msg + "\n";
                }

                else
                {
                    output.Text += msg;
                }
            });
        });
        socket.Start();
        Task task = new Task(() =>
        {
            while (true)
            {
                Console.WriteLine(socket.IsRunning);
                Thread.Sleep(500);
            }
        });
        task.Start();
        
    }

    private void Send(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            socket!.SendInstant(command.Text);

        }
        catch (Exception ex)
        {
            output.Text += "Error: " + ex.Message+"\n";
        }
    }

    private void Disconnect(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        socket!.Dispose();
        cookies = new CookieContainer();
        disconnect.IsEnabled = false;
        host.IsEnabled = true;
        port.IsEnabled = true;
        connectbutton.IsEnabled = true;
        // connecttls.IsEnabled = true;

        command.IsEnabled = false;
        sendbutton.IsEnabled = false;
        pfxReturn.disableCertCheck = false;
        pfxReturn.hasEntry = false;
        output.Text += ("\nDisconnected. \n");
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

    private void Port_OnValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        config.Port = (int)(sender as NumericUpDown)!.Value!;
    }

    private void Username_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
       config.Username = Username.Text;
    }

    private void Password_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        config.Password = password.Text; 
    }
}