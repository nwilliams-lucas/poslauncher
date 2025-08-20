using Microsoft.Win32;
using POSLauncher.Services;
using System.Windows;

namespace POSLauncher
{
    public partial class ConfigurationWindow : Window
    {
        private readonly ConfigurationManager _configManager;

        public ConfigurationWindow(ConfigurationManager configManager)
        {
            InitializeComponent();
            _configManager = configManager;
            LoadCurrentConfiguration();
        }

        private void LoadCurrentConfiguration()
        {
            var config = _configManager.GetConfiguration();
            
            ClientPathTextBox.Text = config.CommerceClientPath;
            ArgumentsTextBox.Text = config.CommerceClientArguments;
            AutoStartCheckBox.IsChecked = config.AutoStartOnBoot;
            TimeoutTextBox.Text = config.ServiceStartTimeoutSeconds.ToString();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Commerce_Client Application",
                Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*",
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ClientPathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var config = _configManager.GetConfiguration();
            
            config.CommerceClientPath = ClientPathTextBox.Text;
            config.CommerceClientArguments = ArgumentsTextBox.Text;
            config.AutoStartOnBoot = AutoStartCheckBox.IsChecked ?? true;
            
            if (int.TryParse(TimeoutTextBox.Text, out int timeout) && timeout > 0)
            {
                config.ServiceStartTimeoutSeconds = timeout;
            }

            _configManager.UpdateCommerceClientSettings(config.CommerceClientPath, config.CommerceClientArguments);
            
            MessageBox.Show("Configuration saved successfully.", "Configuration", 
                          MessageBoxButton.OK, MessageBoxImage.Information);
            
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}