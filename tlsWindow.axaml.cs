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
    static public bool disableCertCheck = false;

    
}


public partial class tlsWindow : Window
{
    private void CheckBox_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(CHECK.IsChecked == true)
        {
            pfxDir.IsEnabled = false;
            password.IsEnabled = false;
            pfxReturn.disableCertCheck = true; 

        }
        if (CHECK.IsChecked == false)
        {
            pfxDir.IsEnabled = true;
            password.IsEnabled = true;
            pfxReturn.disableCertCheck = false;     

        }
    }
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
            pfxDir.Text = files[0].Path.LocalPath;
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
            password.Text = files[0].Path.LocalPath;
        }
    }
    private void okButton(object? sender, RoutedEventArgs e)
    {
        if (!pfxReturn.disableCertCheck)
        {
            pfxReturn.hasEntry = true;
            pfxReturn.Dir = pfxDir.Text;
            pfxReturn.Password = File.ReadAllText(password.Text!);
        }
        
        Close();
    }
    private void cancelButton(object? sender, RoutedEventArgs e)
    {
        pfxReturn.hasEntry = false;
        Close();
    }

}