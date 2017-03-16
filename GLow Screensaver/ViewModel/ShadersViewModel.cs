using GLowCommon.Data;
using System.Collections.ObjectModel;

namespace GLow_Screensaver.ViewModel
{
    /// <summary>
    /// View-model for the shaders.
    /// </summary>
    public class SettingsViewModel
    {
        private ObservableCollection<ShaderViewModel> _shaders = new ObservableCollection<ShaderViewModel>();

        public ObservableCollection<ShaderViewModel> Shaders
        {
            get { return _shaders; }
        }
    }
}
