﻿//
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
using System.Collections.Generic;
using System.ServiceModel;

namespace GLow_Screensaver.Services
{
    /// <summary>
    /// Service to access to the shaders.
    /// </summary>
    public static class ShaderService
    {
        /// <summary>
        /// Return the number of shaders.
        /// </summary>
        /// <returns>The number of shaders.</returns>
        public static int CountShaders()
        {
            ChannelFactory<IShaderService> pipeFactory =
                new ChannelFactory<IShaderService>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/" + ShaderServiceConst.SERVICE_NAME));

            IShaderService pipeProxy = pipeFactory.CreateChannel();

            return pipeProxy.CountShader();
        }

        /// <summary>
        /// Return the list of shaders.
        /// </summary>
        /// <param name="startIndex">Minimum start index of the first shader to return.</param>
        /// <param name="count">Number of shaders to return.</param>
        /// <returns>The list of shaders.</returns>
        public static List<ShaderModel> GetShaders(int startIndex, int count)
        {
            ChannelFactory<IShaderService> pipeFactory =
                new ChannelFactory<IShaderService>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/" + ShaderServiceConst.SERVICE_NAME));

            IShaderService pipeProxy = pipeFactory.CreateChannel();

            return pipeProxy.GetShaders(startIndex, count);
        }

        public static Dictionary<string, string> GetShadersID()
        {
            ChannelFactory<IShaderService> pipeFactory =
                new ChannelFactory<IShaderService>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/" + ShaderServiceConst.SERVICE_NAME));

            IShaderService pipeProxy = pipeFactory.CreateChannel();

            return pipeProxy.GetShadersId();
        }
    }
}
