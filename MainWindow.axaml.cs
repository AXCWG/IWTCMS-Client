using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ws
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            foreach (var config in Singletons.configs)
            {
                TabControl.Items.Add(new TabItem()
                {
                    Content = new MainView(this, config), 
                    Header = config.Header
                     
                });
            }
            

        }

        private void Refresh()
        {
            TabControl.Items.Clear();
            foreach (var config in Singletons.configs)
            {
                TabControl.Items.Add(new TabItem()
                {
                    Content = new MainView(this, config), 
                    Header = config.Header
                     
                });
            }
        }

        private void ShowAbout(object? sender, RoutedEventArgs e)
        {
            
            Window about = new Window()
            {
                Content = new About(),
                Width = 400, Height = 150, Title = "About", ShowInTaskbar = false, CanResize = false,
            };
            about.ShowDialog(this);
        }

        private async void AddConfigMenuItemOnClick(object? sender, RoutedEventArgs e)
        {
            AddConfig addConfig = new AddConfig();
            await addConfig.ShowDialog(this); 
            Refresh();
        }
    }
}