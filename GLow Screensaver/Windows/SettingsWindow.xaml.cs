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

using GLow_Screensaver.Services;
using GLow_Screensaver.ViewModel;
using GLowCommon.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace GLow_Screensaver
{
    /// <summary>
    /// Settings window.
    /// </summary>
    public partial class SettingsWindow : Window
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        public SettingsViewModel ViewModel
        {
            get;
            set;
        }

        private BackgroundWorker _shadersBackgroundWorker = new BackgroundWorker();

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
            Loaded += SettingsWindow_Loaded;
        }
        #endregion

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = new SettingsViewModel();
            DataContext = ViewModel;

            // Get the list of shaders
            List<string> shadersUID = ShaderService.GetShadersUID();

            // Update the list asynchronously
            _shadersBackgroundWorker.DoWork += _shadersBackgroundWorker_DoWork;
            _shadersBackgroundWorker.ProgressChanged += _shadersBackgroundWorker_ProgressChanged;
            _shadersBackgroundWorker.WorkerReportsProgress = true;
            _shadersBackgroundWorker.RunWorkerAsync(shadersUID);

            /*const int NB_SHADERS = 10;

            bool firstShader = true;
            int count = ShaderService.CountShaders();
            if (count > 1)
            {

                for (int s = 0; s < NB_SHADERS; s++)
                {
                    List<ShaderModel> shaders = ShaderService.GetShaders(s, 1);
                    foreach (ShaderModel shader in shaders)
                    {
                        if (shader.ImageSources.Count > 0)
                        {
                            ViewModel.Shaders.Add(new ShaderViewModel(shader));
                            if (firstShader) preview.Source = shader.ImageSources[0].SourceCode;
                            firstShader = false;
                        }
                    }
                }
            }*/
        }

        private void _shadersBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ViewModel.Shaders.Add(new ShaderViewModel((ShaderModel)e.UserState));
        }

        private void _shadersBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            List<string> shadersUID = (List<string>)e.Argument;
            foreach (string shaderUID in shadersUID)
            {
                ShaderModel shader = ShaderService.GetShader(shaderUID);
                _shadersBackgroundWorker.ReportProgress((int)((100.0d * index++) / shadersUID.Count), shader);
            }
        }

        private bool IsUserVisible(FrameworkElement element, FrameworkElement container)
        {
            if (!element.IsVisible)
                return false;

            Rect bounds = element.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            Rect rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            return rect.Contains(bounds.TopLeft) || rect.Contains(bounds.BottomRight);
        }

        internal void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        private void CloseBox_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ReduiceBox_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }

    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_INVALID_STATE = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // ...
        WCA_ACCENT_POLICY = 19
        // ...
    }
}