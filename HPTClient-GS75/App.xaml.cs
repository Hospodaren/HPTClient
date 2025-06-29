using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Security.Permissions;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    //[assembly: IsolatedStorageFilePermission(SecurityAction.RequestOptional, Unrestricted = true)]
    public partial class App : Application
    {
        public static string FileToOpen;
        private void Application_Startup_1(object sender, StartupEventArgs e)
        {
            try
            {
                if (e.Args != null && e.Args.Count(s => s.Contains(".hpt5")) > 0)
                {
                    if (e.Args.Length == 1)
                    {
                        FileToOpen = e.Args.FirstOrDefault(s => s.Contains(".hpt5"));
                    }
                    else
                    {
                        string completeArgs = e.Args.Aggregate((s, next) => s + " " + next);
                        var rexFilename = new Regex(@"\w:[\w\\\.\s_-]+?\.hpt5", RegexOptions.IgnoreCase);
                        if (rexFilename.IsMatch(completeArgs))
                        {
                            Match m = rexFilename.Match(completeArgs);
                            FileToOpen = m.Value;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                // Vetefan hur args funkar här... :-(
            }
        }

        private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //string s = sender.ToString();
        }
    }
}
