using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BeeBurn
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string filename = "BeeBurn_Backup_" + RandomString(4);
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            BeeBurnVM.Get().SaveAll(filename, path);
#if DEBUG
            System.Diagnostics.Debugger.Break();
#endif
            e.Handled = true;
            MessageBox.Show("An error has occured. A backup of your files has been saved on the desktop as '" + filename + ".BeeBurn'. We will attempt to keep going, but restart the app as soon as possible.", "BeeBurn Error!");
        }
    }
}
