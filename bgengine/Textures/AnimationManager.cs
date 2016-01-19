using BGEngine.Textures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine
{
    public class AnimationManager
    {
        private Dictionary<string, Animation2D> animations = new Dictionary<string, Animation2D>();

        public void LoadAnimationsFromJson(string fileName)
        {
            if (!File.Exists(fileName))
                throw new ArgumentException("Could not find file \"" + fileName + "\"");

            string json = File.ReadAllText(fileName);
            var animList = JsonConvert.DeserializeObject<List<Animation2D>>(json);
            foreach (var anim in animList)
            {
                if (animations.ContainsKey(anim.Name))
                {
                    throw new Exception(string.Format("Animation with name {0} already exists", anim.Name));
                }
                animations.Add(anim.Name, anim);
            }
        }

        public Animation2D this[string name]
        {
            get
            {
                return animations[name];
            }
        }
    }
}
