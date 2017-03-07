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

namespace GLowService.Services
{
    /// <summary>
    /// Services for the shaders.
    /// </summary>
    public class ShaderService : IShaderService
    {
        /// <summary>
        /// Returns the list of shaders.
        /// </summary>
        /// <returns>The list.</returns>
        public override List<ShaderModel> GetShaders()
        {
            List<ShaderModel> result = new List<ShaderModel>();
            SQLiteConnection db = Database.Instance.GetConnection();
            IEnumerator<Shader> shaders = (from s in db.Table<Shader>() select s).GetEnumerator();
            while (shaders.MoveNext())
            {
                Shader sourceShader = shaders.Current;
                ShaderModel targetShader = new ShaderModel();
                targetShader.Author = sourceShader.Author;
                targetShader.Description = sourceShader.Description;
                targetShader.Favorite = sourceShader.Favorite;
                targetShader.Id = sourceShader.Id;
                targetShader.LastUpdate = sourceShader.LastUpdate;
                targetShader.Name = sourceShader.Name;
                targetShader.ReadOnly = sourceShader.ReadOnly;
                targetShader.ShadertoyID = sourceShader.ShadertoyID;
                result.Add(targetShader);
            }

            return result;
        }

        /// <summary>
        /// Returns the shader's ID and their name.
        /// </summary>
        /// <returns>The shader's ID and their name.</returns>
        public override Dictionary<string, string> GetShadersId()
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
    }
}
