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

using System.Windows;

namespace GLow_Screensaver.Windows
{
    /// <summary>
    /// Base class used to move window without title bar.
    /// </summary>
    public class WindowBase : Window
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public WindowBase() : base()
        {
            MouseDown += WindowBase_MouseDown;
        }

        /// <summary>
        /// Move the window draging it with the mouse.
        /// </summary>
        /// <param name="sender">The object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void WindowBase_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left) DragMove();
        }
    }
}
