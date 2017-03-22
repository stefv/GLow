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

        private ShaderViewModel _thumbnail;

        public ShaderViewModel Thumbnail
        {
            get { return _thumbnail; }
            set { _thumbnail = value; }
        }

    }
}
