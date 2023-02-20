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
            throw new Exception("BeeBurn has crashed! A backup of your Stacks has been saved on the desktop, as " + filename + ".BeeBurn");
        }
    }
}
