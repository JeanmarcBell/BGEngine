using Newtonsoft.Json;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine.Textures
{
    public class Animation2D
    {
        public string Name;
        public List<Animation2DFrame> Frames;
        
        [JsonIgnore]
        public Animation2DFrame this[int index] { get { return Frames[index]; } }
        [JsonIgnore]
        public int Count { get { return Frames.Count; } }

        public Animation2D(List<Animation2DFrame> frames)
        {
            this.Frames = frames;
        }
    }

    public class Animation2DFrame
    {
        public double DurationMS { get; set; }
        public string AtlasName { get; set; }
        public string TextureName { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }

        [JsonIgnore]
        public Texture2D Texture { get { return ResourceManager.Art[AtlasName, TextureName]; } }
    }
}
