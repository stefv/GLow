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
using GLow_Screensaver.Windows;
using GLow_Screensaver.Windows.ShadertoyJson;
using GLowScreensaver;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;

namespace GLow_Screensaver
{
    /// <summary>
    /// Dialog to download shaders from ShaderToy.
    /// </summary>
    public partial class ShaderDownloadDialog : WindowBase
    {
        #region Constants
        /// <summary>
        /// URL to the JSON services.
        /// </summary>
        private const string SHADERTOY_JSON_URL = "https://www.shadertoy.com/api/v1/shaders";
        #endregion
        #region Attributes
        /// <summary>
        /// The background worker to download the shaders from ShaderToy.
        /// </summary>
        private BackgroundWorker _worker;
        #endregion
        #region Internal classes
        /// <summary>
        /// Data passed to the progress event.
        /// </summary>
        internal class WorkerData
        {
            public int NbShaders { get; set; }
            public Shader ShaderInfo { get; set; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShaderDownloadDialog() : base()
        {
            InitializeComponent();
        }
        #endregion

        #region Start the downloading
        /// <summary>
        /// Start the background worker to download the shaders.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += Worker_DoWork;
            _worker.ProgressChanged += Worker_ProgressChanged;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            _worker.RunWorkerAsync();
        }
        #endregion
        #region Close the dialog when the work is done 
        /// <summary>
        /// Close the window when the download is completed.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SQLiteConnection db = Database.Instance.GetConnection();

            ImageSource source = db.Find<ImageSource>(1);
            this.DialogResult = true;
            Close();
        }

        /// <summary>
        /// Stop the downloading.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            _worker.CancelAsync();
        }
        #endregion
        #region Download the shaders
        /// <summary>
        /// Show the change in the progess.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WorkerData data = (WorkerData)e.UserState;

            progress.Maximum = data.NbShaders;
            progress.Value = e.ProgressPercentage;
            shaderIDText.Text = data.ShaderInfo.ShadertoyID;
            shaderNameText.Text = data.ShaderInfo.Name;
        }

        /// <summary>
        /// Download the shaders.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the connection to the database before to retreive the data from Shadertoy
            SQLiteConnection db = Database.Instance.GetConnection();

            // Load the shader list
            List<string> shaderList = GetShadertoyList();
            if (shaderList == null) return;

            db.BeginTransaction();

            // Load the informations of each shader
            int index = 0;
            foreach (string shaderId in shaderList)
            {
                // If this shader exists already, read its the informations from the database
                Shader shader = (from s in db.Table<Shader>() where s.ShadertoyID.Equals(shaderId) select s).FirstOrDefault();
                if (shader == null)
                {
                    ShaderV1 shadertoy = GetShadertoyShader(shaderId);

                    // Sauvegarde la shader
                    shader = new Shader()
                    {
                        ShadertoyID = shaderId,
                        Name = shadertoy.Shader.info.name,
                        Description = shadertoy.Shader.info.description,
                        Author = shadertoy.Shader.info.username,
                        ReadOnly = true,
                        Favorite = false,
                        Type = "GLSL",
                        LastUpdate = DateTime.Now
                    };
                    db.Insert(shader);

                    // Source garde le source
                    ImageSource imageSource = new ImageSource()
                    {
                        Shader = shader.Id,
                        SourceCode = shadertoy.Shader.renderpass[0].code
                    };
                    db.Insert(imageSource);
                }

                _worker.ReportProgress(++index, new WorkerData() { NbShaders = shaderList.Count, ShaderInfo = shader });
                if (_worker.CancellationPending) break;
            }

            db.Commit();
        }

        /// <summary>
        /// Download the list of shaders from Shadertoy.
        /// </summary>
        /// <returns>The liste of shaders.</returns>
        public List<string> GetShadertoyList()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SHADERTOY_JSON_URL + "?key=" + PrivateData.ShaderToyKey);
            request.Method = "POST";
            request.ContentType = "application/json";

            List<string> shaderIdList = null;
            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string responseText = streamReader.ReadToEnd();
                ListAll obj = JsonConvert.DeserializeObject<ListAll>(responseText);
                shaderIdList = new List<string>(obj.Results);
            }

            return shaderIdList;
        }

        /// <summary>
        /// Download one shader with the given id.
        /// </summary>
        /// <param name="id">The id of the sahder to download.</param>
        /// <returns>The shader downloaded.</returns>
        public ShaderV1 GetShadertoyShader(string id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SHADERTOY_JSON_URL + "/" + id + "?key=" + PrivateData.ShaderToyKey);
            request.Method = "POST";
            request.ContentType = "application/json";

            ShaderV1 shader = null;
            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string responseText = streamReader.ReadToEnd();
                Debug.WriteLine(responseText);
                shader = JsonConvert.DeserializeObject<ShaderV1>(responseText);
                //Now you have your response.
                //or false depending on information in the response
            }

            return shader;
        }
        #endregion
    }
}