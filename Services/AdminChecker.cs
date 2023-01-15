using Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ToggleHypervisor.Services
{
    public class AdminChecker : ServiceBase
    {
        public AdminChecker()
        {
            fileLogger = App.Current.Services.GetService<FileLogger>();
            LogEvent += fileLogger.LogWriteLine;
        }

        private readonly FileLogger fileLogger;

        public void ReRunAsAdmin()
        {

            if (! IsAdmin())
            {
                // re-run the program with administrator privileges
                string exeName = Process.GetCurrentProcess().MainModule.FileName;
                ProcessStartInfo startInfo = new ProcessStartInfo(exeName)
                {
                    Verb = "runas"
                };
                Process.Start(startInfo);
                Application.Current.Shutdown();
            }
        }

        private bool IsAdmin()
        {
            // check if the current user is an administrator
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
