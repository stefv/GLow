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
using GLow_Screensaver.Utils;
using GLow_Screensaver.ViewModel;
using GLowCommon.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

namespace GLow_Screensaver
{
    /// <summary>
    /// Settings window.
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private ObservableCollection<ShaderViewModel> _shaders = new ObservableCollection<ShaderViewModel>();

        public ObservableCollection<ShaderViewModel> Shaders
        {
            get { return _shaders; }
        }

        private ShaderViewModel _thumbnailShader;

        public ShaderViewModel ThumbnailShader
        {
            get { return _thumbnailShader; }
            set { _thumbnailShader = value; }
        }



        public string PreviewSource
        {
            get { return (string)GetValue(PreviewSourceProperty); }
            set { SetValue(PreviewSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviewSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviewSourceProperty =
            DependencyProperty.Register("PreviewSource", typeof(string), typeof(SettingsWindow), new PropertyMetadata(null));



        public string ThumbnailSource
        {
            get { return (string)GetValue(ThumbnailSourceProperty); }
            set { SetValue(ThumbnailSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ThumbnailSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThumbnailSourceProperty =
            DependencyProperty.Register("ThumbnailSource", typeof(string), typeof(SettingsWindow), new PropertyMetadata(null));




        /// <summary>
        /// Background worker to access to the common database.
        /// </summary>
        private BackgroundWorker _shadersBackgroundWorker = new BackgroundWorker();

        /// <summary>
        /// Background worker to create the thumbnails.
        /// </summary>
        private BackgroundWorker _thumbnailBackgroundWorker = new BackgroundWorker();

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
        #endregion
        #region Method to initialize the window
        /// <summary>
        /// Initialize the window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Argument for this event.</param>
        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {

            // Get the list of shaders
            List<string> shadersUID = ShaderService.GetShadersUID();

            // Set the progression in the taskbar
            TaskbarProgress.SetValue(new WindowInteropHelper(Application.Current.MainWindow).Handle, 0, 100);

            // Update the list asynchronously
            _shadersBackgroundWorker.DoWork += _shadersBackgroundWorker_DoWork;
            _shadersBackgroundWorker.ProgressChanged += _shadersBackgroundWorker_ProgressChanged;
            _shadersBackgroundWorker.RunWorkerCompleted += _shadersBackgroundWorker_RunWorkerCompleted;
            _shadersBackgroundWorker.WorkerReportsProgress = true;
            _shadersBackgroundWorker.RunWorkerAsync(shadersUID);
        }
        #endregion
        #region Methods to update the list with a background worker
        /// <summary>
        /// Update the list of shaders using a background worker.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Argument for this event.</param>
        private void _shadersBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            List<string> shadersUID = (List<string>)e.Argument;
            foreach (string shaderUID in shadersUID)
            {
                ShaderModel shader = ShaderService.GetShader(shaderUID);
                if (shader != null) _shadersBackgroundWorker.ReportProgress((int)((100.0d * index++) / shadersUID.Count), shader);
            }
        }

        /// <summary>
        /// Report the progression and update the list control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Argument for this event.</param>
        private void _shadersBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progress.Value = e.ProgressPercentage;

            // Set the progression in the taskbar
            TaskbarProgress.SetValue(new WindowInteropHelper(Application.Current.MainWindow).Handle, progress.Value, 100);

            // TODO Support the iChannel
            // TODO Support the iSampleRate
            // Add the shade only if it doesn't contain iChannel of iSampleRate. These features are not supported yet.
            ShaderModel shaderModel = (ShaderModel)e.UserState;
            ShaderViewModel shaderViewModel = new ShaderViewModel(shaderModel);
            if (!shaderViewModel.SourceCode.Contains("iChannel") && !shaderViewModel.SourceCode.Contains("iSampleRate"))
            {
                Shaders.Add(shaderViewModel);
                if (Shaders.Count == 1)
                {
                    PreviewSource = Shaders[0].SourceCode;
                    shaderList.SelectedIndex = 0;
                    if (shaderViewModel.Thumbnail == null) ThumbnailSource = shaderViewModel.SourceCode;
                }
                nbShaders.Text = "# of shaders: " + Shaders.Count;
            }
        }

        /// <summary>
        /// Finalize the update.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Argument for this event.</param>
        private void _shadersBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progress.Value = 100;

            // Finalize the progression in the taskbar
            TaskbarProgress.SetState(new WindowInteropHelper(Application.Current.MainWindow).Handle, TaskbarProgress.TaskbarStates.NoProgress);
        }
        #endregion

        private bool IsUserVisible(FrameworkElement element, FrameworkElement container)
        {
            if (!element.IsVisible)
                return false;

            Rect bounds = element.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            Rect rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            return rect.Contains(bounds.TopLeft) || rect.Contains(bounds.BottomRight);
        }

        private void scroller_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {

        }
    }
}