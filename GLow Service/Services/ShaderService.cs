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

using GLowCommon.Data;
using GLowCommon.Services;
using GLowService.Data;
using SQLite;
using System.Collections.Generic;
using System;

namespace GLowService.Services
{
    /// <summary>
    /// Services for the shaders.
    /// </summary>
    public class ShaderService : IShaderService
    {
        /// <summary>
        /// Returns the number of shaders.
        /// </summary>
        /// <returns>The nulmber of shaders.</returns>
        public int CountShader()
        {
            SQLiteConnection db = Database.Instance.GetConnection();
            int count = (from s in db.Table<Shader>() select s).Count();
            return count;
        }

        /// <summary>
        /// Get the UID of the shaders. The UID is the ShaderToys ID.
        /// </summary>
        /// <returns>The UID of the shaders.</returns>
        List<string> IShaderService.GetShadersUID()
        {
            //System.Diagnostics.Debugger.Launch();

            List<string> uids = new List<string>();
            SQLiteConnection db = Database.Instance.GetConnection();
            IEnumerator<string> enumUID = (from s in db.Table<Shader>() select s.ShadertoyID).GetEnumerator();
            while (enumUID.MoveNext())
            {
                string uid = enumUID.Current;
                uids.Add(uid);
            }
            return uids;
        }

        /// <summary>
        /// Returns the required shader.
        /// </summary>
        /// <param name="index">Index of the shader in the database.</param>
        /// <returns>The shader or null of the index doesn't exist.</returns>
        public ShaderModel GetShader(string name)
        {
            SQLiteConnection db = Database.Instance.GetConnection();
            Shader shader = (from s in db.Table<Shader>() where s.ShadertoyID == name select s).First();
            return CopyShaderToShadelModel(shader);
        }

        /// <summary>
        /// Returns the list of shaders.
        /// </summary>
        /// <param name="startIndex">Minimum start index of the first shader to return.</param>
        /// <param name="count">Number of shaders to return.</param>
        /// <returns>The list.</returns>
        public List<ShaderModel> GetShaders(int startIndex, int count)
        {
            List<ShaderModel> result = new List<ShaderModel>();
            SQLiteConnection db = Database.Instance.GetConnection();
            IEnumerator<Shader> shaders = (from s in db.Table<Shader>() where s.Id >= startIndex select s).Take(count).GetEnumerator();
            while (shaders.MoveNext())
            {
                Shader sourceShader = shaders.Current;
                ShaderModel targetShader = new ShaderModel();
                targetShader.Author = sourceShader.Author;
                targetShader.Description = sourceShader.Description;
                targetShader.Id = sourceShader.Id;
                targetShader.LastUpdate = sourceShader.LastUpdate;
                targetShader.Name = sourceShader.Name;
                targetShader.ReadOnly = sourceShader.ReadOnly;
                targetShader.ShadertoyID = sourceShader.ShadertoyID;
                result.Add(targetShader);

                // Search image sources
                ImageSource imageSource = (from s in db.Table<ImageSource>() where s.Id == sourceShader.Id select s).FirstOrDefault();
                if (!imageSource.SourceCode.Contains("iChannel") && !imageSource.SourceCode.Contains("iSampleRate"))
                {
                    ImageSourceModel imageSourceTarget = new ImageSourceModel();
                    imageSourceTarget.SourceCode = imageSource.SourceCode;
                    targetShader.ImageSources.Add(imageSourceTarget);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the shader's ID and their name.
        /// </summary>
        /// <returns>The shader's ID and their name.</returns>
        public Dictionary<string, string> GetShadersId()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            SQLiteConnection db = Database.Instance.GetConnection();
            IEnumerator<Shader> shaders = (from s in db.Table<Shader>() select s).GetEnumerator();
            while (shaders.MoveNext())
            {
                Shader sourceShader = shaders.Current;
                result[sourceShader.ShadertoyID] = sourceShader.Name;
            }
            return result;
        }

        /// <summary>
        /// Copy the Shader to the ShaderModel.
        /// </summary>
        /// <param name="shader">The shader to copy.</param>
        /// <returns>The ShaderModel.</returns>
        private ShaderModel CopyShaderToShadelModel(Shader shader)
        {
            ShaderModel shaderModel = null;
            if (shader != null)
            {
                ShaderModel targetShader = new ShaderModel();
                shaderModel.Author = shader.Author;
                shaderModel.Description = shader.Description;
                shaderModel.Id = shader.Id;
                shaderModel.LastUpdate = shader.LastUpdate;
                shaderModel.Name = shader.Name;
                shaderModel.ReadOnly = shader.ReadOnly;
                shaderModel.ShadertoyID = shader.ShadertoyID;

            }
            return shaderModel;
        }
    }
}
