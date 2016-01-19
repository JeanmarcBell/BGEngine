using BGEngine.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine
{
    public sealed class ResourceManager
    {
        #region Singleton
        private static readonly ResourceManager _instance = new ResourceManager();
        private TextureManager _art = new TextureManager();
        private AnimationManager _animations = new AnimationManager();

        static ResourceManager() { }
        private ResourceManager() { }

        public static ResourceManager Instance { get { return _instance; } }
        #endregion

        public static TextureManager Art { get { return Instance._art; } }
        public static AnimationManager Animations { get { return Instance._animations; } }
    }
}
