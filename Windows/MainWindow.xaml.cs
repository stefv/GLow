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

using GLow_Screensaver.Utils;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace GLow_Screensaver
{
    /// <summary>
    /// Main window displaying the screensaver.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        /// <summary>
        /// Get or set the preview mode of the shader control.
        /// </summary>
        //public bool IsPreview
        //{
        //    get { return false; }
        //    set {  }
        //}
        public bool IsPreview
        {
            get { return shaderControl.IsPreview; }
            set { shaderControl.IsPreview = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Open the window with the given coordinates. This constructor is used to display the screen saver on different
        /// screens. The current shader is used.
        /// </summary>
        /// <param name="workingArea">The area where to display the window.</param>
        public MainWindow(Rectangle workingArea) : this()
        {
            // Set the position and the size window to the required coordinates.
#if !DEBUG
            Left = workingArea.Left;
            Top = workingArea.Top;
            Width = workingArea.Width;
            Height = workingArea.Height;
#else
            Width = 640;
            Height = 400;
#endif
            Loaded += NormalModeWindow_Loaded;
        }

        /// <summary>
        /// Open the screensaver in the preview window of the screensaver settings of Windows.
        /// </summary>
        /// <param name="previewWndHandle">The handle of the parent window for the preview.</param>
        public MainWindow(IntPtr previewWndHandle) : this()
        {
            System.Drawing.Rectangle parentRect;
            User32.GetClientRect(previewWndHandle, out parentRect);

            //MessageBox.Show(parentRect.ToString());

            HwndSourceParameters sourceParams = new HwndSourceParameters();
            sourceParams.PositionX = 0;
            sourceParams.PositionY = 0;
            sourceParams.Height = parentRect.Height;
            sourceParams.Width = parentRect.Width;
            sourceParams.ParentWindow = previewWndHandle;
            sourceParams.WindowStyle = (int)(WindowStyles.WS_VISIBLE | WindowStyles.WS_CHILD | WindowStyles.WS_CLIPCHILDREN);

            HwndSource winWPFContent = new HwndSource(sourceParams);
            winWPFContent.Disposed += new EventHandler(winWPFContent_Disposed);
            winWPFContent.RootVisual = grid;

            Loaded += PreviewModeWindow_Loaded;

            shaderControl.InitializeFragmentShader();
        }
        #endregion

        /// <summary>
        /// In preview mode, close the window if the panret window is closed.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Argument for this event.</param>
        private void winWPFContent_Disposed(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Load the current selected shader in the main window.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Argument for this event.</param>
        private void NormalModeWindow_Loaded(object sender, RoutedEventArgs e)
        {
#if !DEBUG
            if (!IsPreview) {
                WindowState = WindowState.Maximized;
                Topmost = true;
            }
#endif
            shaderControl.InitializeFragmentShader();
        }

        /// <summary>
        /// Load the current selected shader in the preview window.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Argument for this event.</param>
        private void PreviewModeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            shaderControl.InitializeFragmentShader();
        }

        /// <summary>
        /// Show the FPS value if the CTRL is pressed.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void NormalModeWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl) shaderControl.IsShowFPS = true;
            else Application.Current.Shutdown();
        }

        /// <summary>
        /// Hide the FPS value if the CTRL is pressed.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void NormalModeWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl) shaderControl.IsShowFPS = false;
        }
    }
}