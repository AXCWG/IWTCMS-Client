using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;

namespace ws;

public partial class tlsWindow : Window
{
    public static FilePickerFileType pfx
    {
        get;
    } = new("pfx")
    {
        Patterns = ["*.pfx"],
        AppleUniformTypeIdentifiers = ["com.rsa.pkcs-12"],
        MimeTypes = ["application/x-pkcs12"],
    };
    public tlsWindow()
    {
        InitializeComponent();
    }
    private async void pfxDirFinder(object? sender, RoutedEventArgs e)
    {
        var toplevel = TopLevel.GetTopLevel(this);
        var files = await toplevel!.StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions
        {
            Title = "*.pfx",
            AllowMultiple = false,
            FileTypeFilter = [pfx]
        });
        if (files.Count == 1){
            pfxDir.Text = files[0].Path.AbsolutePath;
        }
    }
    private void passwordFinder(object? sender, RoutedEventArgs e)
    {

    }
}