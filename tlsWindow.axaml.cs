using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;

namespace ws;
public static class pfxReturn
{
    static public bool hasEntry = false;
    static public string? Dir { get; set; }
    static public string? Password { get; set; }

}


public partial class tlsWindow : Window
{

    public static FilePickerFileType pfx
    {
        get;
    } = new("PFX File")
    {
        Patterns = ["*.pfx"],
        AppleUniformTypeIdentifiers = ["com.rsa.pkcs-12"],
        MimeTypes = ["application/x-pkcs12"],
    };
    public static FilePickerFileType txt
    {
        get;
    } = new("Text Document")
    {
        Patterns = ["*.txt"],
        AppleUniformTypeIdentifiers = ["public.plain-text"],
        MimeTypes = ["text/plain"],
    };
    public tlsWindow()
    {
        InitializeComponent();
    }

    private async void pfxDirFinder(object? sender, RoutedEventArgs e)
    {
        var toplevel = TopLevel.GetTopLevel(this);
        var files = await toplevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "*.pfx",
            AllowMultiple = false,
            FileTypeFilter = [pfx]
        });
        if (files.Count == 1)
        {
            pfxDir.Text = files[0].Path.AbsolutePath;
        }

    }
    private async void passwordFinder(object? sender, RoutedEventArgs e)
    {
        var toplevel = TopLevel.GetTopLevel(this);
        var files = await toplevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Password",
            AllowMultiple = false,
            FileTypeFilter = [txt]
        });
        if (files.Count == 1)
        {
            password.Text = files[0].Path.AbsolutePath;
        }
    }
    private void okButton(object? sender, RoutedEventArgs e)
    {
        pfxReturn.hasEntry = true;
        pfxReturn.Dir = pfxDir.Text;
        pfxReturn.Password = File.ReadAllText(password.Text!);
        Close();
    }
    private void cancelButton(object? sender, RoutedEventArgs e)
    {
        pfxReturn.hasEntry = false;
        Close();
    }

}