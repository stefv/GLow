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
using GLowService.Helper;
using SQLite;
using System.Collections.Generic;
using System.ServiceModel;

namespace GLowService.Services
{
    /// <summary>
    /// Services for the shaders.
    /// </summary>
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class ShaderService : IShaderService
    {
        /// <summary>
        /// Returns the number of shaders.
        /// </summary>
        /// <returns>The number of shaders.</returns>
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
            List<string> uids = new List<string>();
            SQLiteConnection db = Database.Instance.GetConnection();
            IEnumerator<Shader> shaders = (from s in db.Table<Shader>() select s).GetEnumerator();
            while (shaders.MoveNext())
            {
                Shader shader = shaders.Current;
                uids.Add(shader.ShadertoyID);
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
            Shader shader = (from s in db.Table<Shader>() where s.ShadertoyID == name select s).FirstOrDefault();
            return CopyShaderToShadelModel(shader);
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
                shaderModel = new ShaderModel();
                shaderModel.Author = shader.Author;
                shaderModel.Description = shader.Description;
                shaderModel.Id = shader.Id;
                shaderModel.LastUpdate = shader.LastUpdate;
                shaderModel.Name = shader.Name;
                shaderModel.ReadOnly = shader.ReadOnly;
                shaderModel.ShadertoyID = shader.ShadertoyID;

                // Search image sources. This version of GLow doesn't support the shaders with iChannel and iSampleRate.
                // Maybe for the next versions ?
                SQLiteConnection db = Database.Instance.GetConnection();
                IEnumerator<ImageSource> imageSources = (from s in db.Table<ImageSource>() where s.Id == shader.Id select s).GetEnumerator();
                while (imageSources.MoveNext())
                {
                    ImageSource imageSource = imageSources.Current;
                    ImageSourceModel imageSourceTarget = new ImageSourceModel();
                    imageSourceTarget.Id = imageSource.Id;
                    imageSourceTarget.SourceCode = imageSource.SourceCode;
                    shaderModel.ImageSources.Add(imageSourceTarget);
                }
                LogHelper.Info(101, "Shader " + shaderModel.ShadertoyID + ": #sources" + shaderModel.ImageSources.Count);
            }
            return shaderModel;
        }
    }
}
