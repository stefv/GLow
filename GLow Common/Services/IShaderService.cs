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
using System.Collections.Generic;
using System.ServiceModel;

namespace GLowCommon.Services
{
    /// <summary>
    /// Service interface to access to the database.
    /// </summary>
    [ServiceContract]
    public interface IShaderService
    {
        /// <summary>
        /// Return the number of shaders.
        /// </summary>
        /// <returns>The number of shaders.</returns>
        [OperationContract]
        int CountShader();

        /// <summary>
        /// Get the shader's ID and their name.
        /// </summary>
        /// <returns>The shader's ID and their name.</returns>
        [OperationContract]
        Dictionary<string, string> GetShadersId();

        /// <summary>
        /// Returns the list of shaders from ShaderToy.
        /// </summary>
        /// <returns>The list.</returns>
        [OperationContract]
        List<ShaderModel> GetShaders();
    }

    /// <summary>
    /// Class of constants for the service.
    /// </summary>
    public sealed class ShaderServiceConst
    {
        /// <summary>
        /// Name of the service.
        /// </summary>
        public const string SERVICE_NAME = "PipeGlowService";
    }
}
