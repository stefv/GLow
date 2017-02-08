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
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace GLow_Screensaver.Windows.Converters
{
    /// <summary>
    /// Remove the special characters from the description.
    /// </summary>
    public class DescriptionConverter : IValueConverter
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
            string text = WebUtility.HtmlDecode(value.ToString());
            text = Regex.Replace(text, "<br[ ]*/>", "\r\n");
            text = Regex.Replace(text, "<[^>]*>", "");
            return text;
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
