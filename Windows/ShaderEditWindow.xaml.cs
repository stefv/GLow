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

using GLow_Screensaver.Windows;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml;

namespace GLow_Screensaver
{
    /// <summary>
    /// Window to edit the source code of a shader.
    /// </summary>
    public partial class ShaderEditWindow : WindowBase
    {
        #region Properties
        /// <summary>
        /// Source code.
        /// </summary>
        public string Code
        {
            get { return editor.Text; }
            set { editor.Text = value; }
        }

        /// <summary>
        /// true if the code is readonly. 
        /// </summary>
        public bool IsReadOnly
        {
            get { return editor.IsReadOnly; }
            set { editor.IsReadOnly = value; UpdateStatusReadonly(); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShaderEditWindow() : base()
        {
            InitializeComponent();

            // Set the color of the syntax highlighting
            Stream hlslSyntax = Assembly.GetCallingAssembly().GetManifestResourceStream("GLow_Screensaver.xshd.GLSL.xshd");
            using (XmlTextReader reader = new XmlTextReader(hlslSyntax))
            {
                editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }
        #endregion

        #region Update the status bar and the menu in function of the readonly flag
        /// <summary>
        /// Update the readonly status and the menus.
        /// </summary>
        private void UpdateStatusReadonly()
        {
            if (IsReadOnly)
            {
                statusReadonly.Text = "Read-only";
                menuPaste.Visibility = Visibility.Collapsed;
                menuCut.Visibility = Visibility.Collapsed;
            }
            else
            {
                statusReadonly.Text = "Read/Write";
                menuPaste.Visibility = Visibility.Visible;
                menuCut.Visibility = Visibility.Visible;
            }
        }
        #endregion
        #region Menu items
        /// <summary>
        /// Close the window.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void MenuItemQuit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Select all the code.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void MenuItemSelectAll_Click(object sender, RoutedEventArgs e)
        {
            editor.SelectAll();
        }

        /// <summary>
        /// Copy the selected text to the clipboard.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void MenuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            editor.Copy();
        }
        #endregion
    }
}