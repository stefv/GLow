using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace GLowService
{
    public class ShaderDownloader
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShaderDownloader()
        {
        }

        public void download()
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
    }
}
