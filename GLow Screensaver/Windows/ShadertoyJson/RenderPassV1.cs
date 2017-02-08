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

namespace GLow_Screensaver.Windows.ShadertoyJson
{
    /// <summary>
    /// Render pass informations.
    /// </summary>
    public class RenderPassV1
    {
        //        "inputs":[{
        //				"id":30,
        //				"src":"\/presets\/tex16.png",
        //				"ctype":"texture",
        //				"channel":0,
        //				"sampler":{
        //					"filter":"mipmap",
        //					"wrap":"repeat",
        //					"vflip":"false",
        //					"srgb":"false",
        //					"internal":"byte"}
        //}
        //			],
        //"outputs":[{
        //	"channel":"0",
        //	"dst":"-1"}
        //],

        /// <summary>
        /// Source code of the shader.
        /// </summary>
        public string code { get; set; }

        //     "name":"",
        //"description":"",
        //"type":"image"}

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RenderPassV1()
        {
        }
    }
}
