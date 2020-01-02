using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TextEncryptionPet.Assemblies;
using TextEncryptionPet.Settings;

namespace TextEncryptionPet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool load = false;
        public MainWindow()
        {
            InitializeComponent();
            foreach (var p in MainSettings.TextProcessors)
            {
                Processors_listBox.Items.Add(new TextBox().Text = p.ProcessorName);
            }
            Processors_listBox.SelectedItem = Processors_listBox.Items[0];
            Encryption_key.Text = Encryption.GenerateKey(new Random().Next(), 16);
            ErrorLog.Background = (Brush)new BrushConverter().ConvertFrom("#222");
            load = true;
        }

        private void SettingsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Output.Text = GetIndex().ToString();
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            Encrypt(Input.Text);
        }

        private void Output_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (MainSettings.TextProcessors.Count >= 0 && GetIndex() >= 0)
                {
                    MainSettings.TextProcessors[GetIndex()].SetKey(Encryption_key.Text);
                    Input.Text = MainSettings.TextProcessors[GetIndex()].DecryptText(Output.Text);
                    ErrorLog.Background = (Brush)new BrushConverter().ConvertFrom("#222");
                    ErrorLog_Text.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorLog_Text.Text = ex.Message;
                ErrorLog.Background = (Brush)new BrushConverter().ConvertFrom("#FF2323");
            }
        }

        private void Encryption_key_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!load) return;
            Encrypt(Input.Text);
        }

        private void Togle_processors(object sender, RoutedEventArgs e)
        {
            if (Processors_list.Width != new GridLength(0))
                Processors_list.Width = new GridLength(0);
            else
                Processors_list.Width = new GridLength(200);
        }

        private int GetIndex() => Processors_listBox.Items.IndexOf(Processors_listBox.SelectedItem);

        private void New_Key(object sender, RoutedEventArgs e)
        {
            Encryption_key.Text = Encryption.GenerateKey(new Random().Next(), 16);
        }

        private void Generate(object sender, RoutedEventArgs e)
        {
            Encrypt(Input.Text);
        }

        private void Encrypt(string text)
        {
            try
            {
                if (MainSettings.TextProcessors.Count >= 0 && GetIndex() >= 0)
                {
                    MainSettings.TextProcessors[GetIndex()].SetKey(Encryption_key.Text);
                    Output.Text = MainSettings.TextProcessors[GetIndex()].EncryptText(text);
                    ErrorLog.Background = (Brush)new BrushConverter().ConvertFrom("#222");
                    ErrorLog_Text.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorLog_Text.Text = ex.Message;
                ErrorLog.Background = (Brush)new BrushConverter().ConvertFrom("#FF2323");
            }
        }
    }
}
