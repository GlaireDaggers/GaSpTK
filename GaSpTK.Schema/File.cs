using System.Collections.Generic;

namespace GaSpTK.Schema
{
    public partial class File
    {
        public File()
        {
            Animation = new List<SpriteAnim>();
            Atlas = new List<SpriteAtlas>();
            Event = new List<EventInfo>();
            Metadata = new List<MetaPropInfo>();
        }
    }

    public partial class SpriteAnim
    {
        public SpriteAnim()
        {
            Events = new List<Event>();
            Metadata = new List<Metadatum>();
            RectLayers = new List<RectLayer>();
            SpriteLayers = new List<SpriteLayer>();
        }
    }

    public partial class Metadatum
    {
        public Metadatum()
        {
            Data = new Dictionary<string, object>();
        }
    }

    public partial class RectLayer
    {
        public RectLayer()
        {
            Frames = new List<RectLayerFrame>();
        }
    }

    public partial class SpriteLayer
    {
        public SpriteLayer()
        {
            Frames = new List<SpriteLayerFrame>();
        }
    }

    public partial class SpriteAtlas
    {
        public SpriteAtlas()
        {
            Sprites = new List<Sprite>();
        }
    }

    public partial class EventInfo
    {
        public EventInfo()
        {
            Params = new List<Param>();
        }
    }
}