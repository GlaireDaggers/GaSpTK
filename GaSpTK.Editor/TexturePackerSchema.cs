using System.Collections.Generic;

namespace GaSpTK.Editor
{
    public struct TexturePackerRect
    {
        public int x;
        public int y;
        public int w;
        public int h;
    }

    public struct TexturePackerSize
    {
        public int w;
        public int h;
    }

    public struct TexturePackerFrame
    {
        public TexturePackerRect frame;
        public bool rotated;
        public bool trimmed;
        public TexturePackerRect spriteSourceSize;
        public TexturePackerSize sourceSize;
    }

    public struct TexturePackerMeta
    {
        public string app;
        public string version;
        public string image;
        public string format;
        public TexturePackerSize size;
        public string scale;
        public string smartupdate;
    }

    public class TexturePackerFile
    {
        public Dictionary<string, TexturePackerFrame> frames = new Dictionary<string, TexturePackerFrame>();
        public TexturePackerMeta meta;
    }
}