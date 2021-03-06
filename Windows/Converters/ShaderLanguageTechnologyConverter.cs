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
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GLow_Screensaver.Windows.Converters
{
    /// <summary>
    /// Return a logo of the technology used for the shader (OpenGL or DirectX).
    /// </summary>
    public class ShaderLanguageTechnologyConverter : IValueConverter
    {
        /// <summary>
        /// The converter.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter to use to convert.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The value converted.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            ImageSource result = new BitmapImage(new Uri("/GLow Screensaver;component/Images/opengl.png", UriKind.Relative));
            return result;
        }

        /// <summary>
        /// The back converter.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter to use to convert.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The value converted.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
