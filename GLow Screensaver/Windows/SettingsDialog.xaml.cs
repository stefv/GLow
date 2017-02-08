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

using GLow_Screensaver.Data;
using GLow_Screensaver.Utils;
using GLow_Screensaver.Windows;
using GLowScreensaver;
using Microsoft.Win32;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace GLow_Screensaver
{
    /// <summary>
    /// Dialog displaying the setting of GLow.
    /// </summary>
    public partial class SettingsDialog : WindowBase
    {
        #region Attributes
        /// <summary>
        /// Filtered list of shaders.
        /// </summary>
        private CollectionViewSource _filteredList;
        #endregion
        #region Command to add or remove favorites
        /// <summary>
        /// Command to add or remove a favorite.
        /// </summary>
        public ICommand AddFavoriteCommand { get; private set; }

        /// <summary>
        /// Class to add or remove favorite.
        /// </summary>
        public class AddFavoriteClick : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            /// <summary>
            /// Add or remove the favorite.
            /// </summary>
            /// <param name="parameter">Parameter for this command.</param>
            public void Execute(object parameter)
            {
                SettingsDialog dialog = (SettingsDialog)parameter;
                if ((dialog.listBox.SelectedItem != null) && (dialog.listBox.SelectedItem is Shader))
                {
                    SQLiteConnection db = Database.Instance.GetConnection();
                    Shader shader = (Shader)dialog.listBox.SelectedItem;
                    shader.Favorite = !shader.Favorite;

                    db.BeginTransaction();
                    db.Update(shader);
                    db.Commit();
                }
            }
        }
        #endregion
        #region Command to show the source code
        /// <summary>
        /// Command to show the source code.
        /// </summary>
        public ICommand SourceCodeCommand { get; private set; }

        /// <summary>
        /// Class to show the source code.
        /// </summary>
        public class SourceCodeClick : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            /// <summary>
            /// Show the source code.
            /// </summary>
            /// <param name="parameter">Parameter for this command.</param>
            public void Execute(object parameter)
            {
                SettingsDialog dialog = (SettingsDialog)parameter;
                if ((dialog.listBox.SelectedItem != null) && (dialog.listBox.SelectedItem is Shader))
                {
                    // Get the source code
                    Shader shader = (Shader)dialog.listBox.SelectedItem;
                    if (shader == null) return;
                    dialog.ShowSourceCode(shader);
                }
            }
        }
        #endregion
        #region Properties
        /// <summary>
        /// Return the list of shaders.
        /// </summary>
        public ObservableHashSet<Shader> ShaderList
        {
            get;
            private set;
        }

        /// <summary>
        /// Return the version of the application.
        /// </summary>
        public string Version
        {
            get
            {
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return version;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SettingsDialog() : base()
        {
            InitializeComponent();
            listBox.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            DataContext = this;
            AddFavoriteCommand = new AddFavoriteClick();
            SourceCodeCommand = new SourceCodeClick();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize the window and load the selected shader.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShaderList = new ObservableHashSet<Shader>();

            _filteredList = new CollectionViewSource();
            _filteredList.Source = ShaderList;
            _filteredList.Filter += FilteredList_Filter;
            listBox.ItemsSource = _filteredList.View;
            RefreshList();

            // Select the current shader
            RegistryKey folder = Registry.CurrentUser.CreateSubKey(@"Software\GLow Screensaver\");
            if (folder.GetValue("ShaderId") != null)
            {
                int shaderId = (int)folder.GetValue("ShaderId");
                foreach (Shader shader in ShaderList)
                {
                    if (shader.Id == shaderId)
                    {
                        listBox.SelectedItem = shader;
                        listBox.ScrollIntoView(listBox.SelectedItem);
                        break;
                    }
                }
            }
            else if (ShaderList.Count > 0) listBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Refresh the list of shaders with the data from the database.
        /// </summary>
        private void RefreshList()
        {
            SQLiteConnection db = Database.Instance.GetConnection();
            IEnumerator<Shader> shaders = (from s in db.Table<Shader>() select s).GetEnumerator();
            while (shaders.MoveNext())
            {
                if (!ShaderList.Contains(shaders.Current))
                {
                    // Check if the shader is using unimplemented features to filter
                    ImageSource source = (from s in db.Table<ImageSource>() where s.Id == shaders.Current.Id select s).FirstOrDefault();
                    if (!source.SourceCode.Contains("iChannel") && !source.SourceCode.Contains("iSampleRate")) ShaderList.Add(shaders.Current);
                }
            }
        }
        #endregion
        #region Filter the list
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilteredList_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is Shader)
            {
                Shader shader = (Shader)e.Item;
                string searchText = textBoxSearch.Text.ToLower();
                if (searchText.Trim() != "")
                {
                    if (shader.Name.ToLower().Contains(searchText))
                    {
                        e.Accepted = true;
                        return;
                    }
                    else if (shader.Description.ToLower().Contains(searchText))
                    {
                        e.Accepted = true;
                        return;
                    }
                } else
                {
                    e.Accepted = true;
                    return;
                }
            }

            e.Accepted = false;
        }

        private void textBoxSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _filteredList.View.Refresh();
        }
        #endregion
        #region Buttons
        /// <summary>
        /// Open a dialog box to update the shader list.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            ShaderDownloadDialog dialog = new ShaderDownloadDialog() { Owner = this };
            bool? result = dialog.ShowDialog();
            if (result.HasValue && result.Value) RefreshList();
        }

        /// <summary>
        /// Save the settings and close the window.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                Shader shader = (Shader)listBox.SelectedItem;
                RegistryKey folder = Registry.CurrentUser.CreateSubKey(@"Software\GLow Screensaver\");
                folder.SetValue("ShaderId", shader.Id);
            }
            Close();
        }

        /// <summary>
        /// Cancel the change and close the window.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e"></param>
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Open the website in the current browser.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        /// <summary>
        /// Reduice the window.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void buttonWindowReduice_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Cancel the action and close the window.
        /// </summary>
        /// <param name="sender">Object for this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void buttonWindowCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Change the states of the buttons in function of the selected shader.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for tis event.</param>
        private void listBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SQLiteConnection db = Database.Instance.GetConnection();
            Shader shader = (Shader)listBox.SelectedItem;
            if (shader != null)
            {
                // Set the shader for the preview
                ImageSource source = (from s in db.Table<ImageSource>() where s.Id == shader.Id select s).FirstOrDefault();
                if (source != null) preview.InitializeFragmentShader(source.SourceCode);

                // Enable the buttons
                //buttonDuplicate.IsEnabled = true; //  FIXME Manage the event
                buttonViewSourceCode.IsEnabled = true;
            }
            else
            {
                // Disable the buttons
                buttonDuplicate.IsEnabled = false;
                buttonViewSourceCode.IsEnabled = false;
            }
        }

        /// <summary>
        /// Change the state of the menus in function of the selected shader.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void ContextMenu_ContextMenuOpening(object sender, System.Windows.Controls.ContextMenuEventArgs e)
        {
            Shader shader = (Shader)listBox.SelectedItem;
            if (shader != null)
            {
                if (shader.Favorite)
                {
                    addFavoriteMenu.Visibility = Visibility.Collapsed;
                    removeFavoriteMenu.Visibility = Visibility.Visible;
                }
                else
                {
                    addFavoriteMenu.Visibility = Visibility.Visible;
                    removeFavoriteMenu.Visibility = Visibility.Collapsed;
                }

                sourceCodeMenu.IsEnabled = true;
            }
            else sourceCodeMenu.IsEnabled = false;

            e.Handled = false;
        }


        #endregion
        #region Show the source code
        /// <summary>
        /// Show the source code of the selected shader.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void buttonViewSourceCode_Click(object sender, RoutedEventArgs e)
        {
            // Get the source code
            Shader shader = (Shader)listBox.SelectedItem;
            if (shader == null) return;
            ShowSourceCode(shader);
        }

        /// <summary>
        /// Show the source code of the given Shader.
        /// </summary>
        /// <param name="shader">The shader with the source code.</param>
        private void ShowSourceCode(Shader shader)
        {
            // Retrieve the source code
            SQLiteConnection db = Database.Instance.GetConnection();
            ImageSource source = (from s in db.Table<ImageSource>() where s.Id == shader.Id select s).FirstOrDefault();

            ShaderEditWindow editWindow = new ShaderEditWindow() { Owner = this };
            editWindow.Code = source.SourceCode;
            editWindow.IsReadOnly = true;
            editWindow.ShowDialog();
        }
        #endregion
    }
}