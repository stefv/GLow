using GLowCommon.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLow_Screensaver.ViewModel
{
    /// <summary>
    /// View-model for the shaders.
    /// </summary>
    public class ShaderViewModel
    {
        private ObservableCollection<ShaderModel> _shaders = new ObservableCollection<ShaderModel>();

        public ObservableCollection<ShaderModel> Shaders
        {
            get { return _shaders; }
        }
    }
}
