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

using System.Collections.Generic;

namespace GLowService.ShadertoyJson
{
    /// <summary>
    /// The root object of the shader.
    /// </summary>
    public class ShaderRoot
    {
        /// <summary>
        /// The version number.
        /// </summary>
        public string ver { get; set; }

        /// <summary>
        /// The informations of the shader.
        /// </summary>
        public ShaderInfoV1 info { get; set; }

        /// <summary>
        /// The render pass informations.
        /// </summary>
        public List<RenderPassV1> renderpass { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShaderRoot()
        {
        }
    }
}