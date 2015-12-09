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

using System.Windows.Controls;

namespace GLow_Screensaver.Controls
{
    /// <summary>
    /// Control to preview a shader.
    /// </summary>
    public partial class PreviewControl : UserControl
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public PreviewControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Method to set the shader to preview
        /// <summary>
        /// Set the shader to preview with the given source code.
        /// </summary>
        /// <param name="code">Source code of the shader to preview.</param>
        public void InitializeFragmentShader(string code)
        {
            preview.InitializeFragmentShader(code);
        }
        #endregion
    }
}
