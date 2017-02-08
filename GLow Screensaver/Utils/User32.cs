﻿//
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
using System.Runtime.InteropServices;

namespace GLow_Screensaver.Utils
{
    /// <summary>
    /// Access functions to user32.dll.
    /// </summary>
    public sealed class User32
    {
        /// <summary>
        /// Get the client rectangle of a window.
        /// </summary>
        /// <param name="hWnd">The handle of the window.</param>
        /// <param name="lpRect">The client rectangle of this window.</param>
        /// <returns>true if the call worked.</returns>
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out System.Drawing.Rectangle lpRect);
    }
}
