using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Threading;
using System.Text.RegularExpressions;

namespace HubHelper
{
    public partial class MainWindow : Window
    {

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

       
        private bool FLAG { get; set; }
        private Thread Job { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            FLAG = true;
        }

        private void TextBox_Raw_TextChanged(object sender, TextChangedEventArgs e)
        {
            var string_tab = TextBox_Raw.Text.Split('.');
            string result = "";
       
            if (TextBox_Raw.Text.Length > 1)
            {
                foreach (string tmp in string_tab)
                {
                    List<string> lista = new List<string>();
                    var tab = tmp.Split(' ');
                    for(int i = 0; i < tab.Length; i++)
                    {
                        if (tab[i] != "")
                        {
                            lista.Add(tab[i]);
                        }
                    }
                
                    for(int i = 0; i < lista.Count; i++)
                    {
                        if (i == 0) 
                        {
                            result = result + char.ToUpper(lista[i][0]) + lista[i].Substring(1).ToLower() + " ";
                        }
                        else
                        {
                            if (i != lista.Count - 1)
                            {
                                result = result + lista[i].ToLower() + " ";
                            }
                            else
                            {
                                result = result + lista[i].ToLower() + ". ";
                            }
                        }                       
                    }
                }
            }
            else
            {
                result = TextBox_Raw.Text.ToUpper();
            }

            TexBox_Result.Text = result;

        }

        private void TurnOffCapsLock()
        {
            if (Keyboard.GetKeyStates(Key.CapsLock) == KeyStates.Toggled)
            {
                const int KEYEVENTF_EXTENDEDKEY = 0x1;
                const int KEYEVENTF_KEYUP = 0x2;
                keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
                keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP,
                (UIntPtr)0);
            }
        }


        private void Check()
        {
            while (FLAG)
            {
                int time = 0;
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (TextBox_Sekundy.Text.Length == 0)
                        {
                            time = 1;
                        }
                        else
                        {
                            time = Convert.ToInt32(TextBox_Sekundy.Text);
                            TurnOffCapsLock();
                        }
                    });
                    Thread.Sleep(time * 1000);
                }
                catch 
                {
                    FLAG = false;
                };
            }           
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_CapsLock_Click(object sender, RoutedEventArgs e)
        {

            if (Button_CapsLock.Content is "Start")
            {
                FLAG = true;
                Job = new Thread(new ThreadStart(Check));
                Job.Start();
                Button_CapsLock.Content = "Stop";
                Stan.Fill = Brushes.Green;
            }
            else if(Button_CapsLock.Content is "Stop")
            {
                FLAG = false;
                Job.Abort();              
                Job = null;
                Stan.Fill = Brushes.Red;
                Button_CapsLock.Content = "Start";
            }
            
        }
    }
}
