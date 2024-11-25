using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ws;

public partial class AddConfig : Window
{
    public AddConfig()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        Singletons.configs.Add(new Config()
        {
            Header = Header.Text!
        });
        Close();
    }
}