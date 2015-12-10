//
// GLow screensaver
// Copyright(C) Stéphane VANPOPERYNGHE
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or(at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
//
using System;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace GLow_Screensaver
{
    /// <summary>
    /// The application.
    /// </summary>
    public partial class App : System.Windows.Application
    {
        /// <summary>
        /// The startup of the application. This method will get the command line arguments and will adapt the action
        /// in function of them.
        /// </summary>
        /// <param name="sender">The objet sending the event.</param>
        /// <param name="e">Argument for this event.</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Get the parameters
            string firstArg = null;
            string secondArg = null;
            if (e.Args.Length >= 1) firstArg = e.Args[0];
            if (e.Args.Length >= 2) secondArg = e.Args[1];

            // Display the preview
            if ((firstArg != null) && (firstArg.ToUpper() == "/P") && (secondArg != null))
            {
                IntPtr previewWndHandle = new IntPtr(long.Parse(secondArg));
                MainWindow _window = new MainWindow(previewWndHandle) { IsPreview = true };
            }
            // Display the screensaver
            else if ((firstArg == null) || (firstArg.ToUpper() == "/S"))
            {
                // Display the screensaver on each screen
                foreach (Screen screen in Screen.AllScreens)
                {
                    MainWindow window = new MainWindow(screen.WorkingArea) { IsPreview = false };
                    window.Show();
                }
            }
            // Show the settings dialog
            else if ((firstArg.ToUpper() == "/C") || ((firstArg.ToUpper().StartsWith("/C:"))))
            {
                new SettingsDialog().ShowDialog();
            }
            // Just quit the application
            else
            {
                App.Current.Shutdown();
            }
        }
    }
}