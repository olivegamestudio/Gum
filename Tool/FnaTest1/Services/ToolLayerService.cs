using RenderingLibrary;
using RenderingLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gum.Managers
{
    public class ToolLayerService
    {
        private SystemManagers _systemManagers;

        public Layer TopLayer { get; private set; }

        public void Initialize(SystemManagers systemManagers)
        {
            _systemManagers = systemManagers;
            TopLayer = _systemManagers.Renderer.AddLayer();
        }

        public void Activity()
        {
            // just in case another plugin adds more layers, keep this one on top:
            if(_systemManagers.Renderer.Layers.Last() != TopLayer)
            {
                _systemManagers.Renderer.RemoveLayer(TopLayer);
                _systemManagers.Renderer.AddLayer(TopLayer);
            }
        }
    }
}
