using Newtonsoft.Json;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using GaSpTK.Schema;

namespace GaSpTK.Editor
{
    public delegate void EditorObjectModifiedHandler();

    public abstract class EditorObject : ReactiveObject
    {
        public event EditorObjectModifiedHandler? OnModified;

        protected EditorObject? parent;

        public EditorObject(EditorObject? parent = null)
        {
            this.parent = parent;
        }

        protected void RaiseModified()
        {
            OnModified?.Invoke();
            parent?.RaiseModified();
        }
    }

    public class EditorDocument : EditorObject
    {
        /// <summary>
        /// Collection of sprite animations defined in this file
        /// </summary>
        public ObservableCollection<SpriteAnim> Animation { get; set; }

        /// <summary>
        /// Collection of sprite atlas definitions referenced by animations in this file
        /// </summary>
        public ObservableCollection<EditorSpriteAtlas> Atlas { get; set; }

        /// <summary>
        /// Collection of information about events which may be triggered by animations in this file
        /// </summary>
        public ObservableCollection<EventInfo> Event { get; set; }

        /// <summary>
        /// Information about properties which can be defined in an animation's metadata
        /// </summary>
        public ObservableCollection<MetaPropInfo> Metadata { get; set; }

        public EditorDocument(File file) : base()
        {
            Animation = new ObservableCollection<SpriteAnim>();
            Atlas = new ObservableCollection<EditorSpriteAtlas>();
            Event = new ObservableCollection<EventInfo>();
            Metadata = new ObservableCollection<MetaPropInfo>();

            foreach (var anim in file.Animation)
            {
                Animation.Add(anim);
            }

            foreach (var atlas in file.Atlas)
            {
                Atlas.Add(new EditorSpriteAtlas(atlas, this));
            }

            foreach (var evt in file.Event)
            {
                Event.Add(evt);
            }

            foreach (var metaprop in file.Metadata)
            {
                Metadata.Add(metaprop);
            }
        }

        public string ToJson()
        {
            File f = new File();

            foreach (var anim in Animation)
            {
                f.Animation.Add(anim);
            }

            foreach (var atlas in Atlas)
            {
                f.Atlas.Add(atlas.ToSpriteAtlas());
            }

            foreach (var evt in Event)
            {
                f.Event.Add(evt);
            }

            foreach (var metaprop in Metadata)
            {
                f.Metadata.Add(metaprop);
            }

            return f.ToJson();
        }
    }

    public class EditorSprite : EditorObject
    {
        private string _id = "";
        private long _x;
        private long _y;
        private long _width;
        private long _height;

        public string Id
        {
            get => _id;
            set
            {
                this.RaiseAndSetIfChanged(ref _id, value);
                this.RaiseModified();
            }
        }

        public long X
        {
            get => _x;
            set
            {
                this.RaiseAndSetIfChanged(ref _x, value);
                this.RaiseModified();
            }
        }

        public long Y
        {
            get => _y;
            set
            {
                this.RaiseAndSetIfChanged(ref _y, value);
                this.RaiseModified();
            }
        }

        public long Width
        {
            get => _width;
            set
            {
                this.RaiseAndSetIfChanged(ref _width, value);
                this.RaiseModified();
            }
        }

        public long Height
        {
            get => _height;
            set
            {
                this.RaiseAndSetIfChanged(ref _height, value);
                this.RaiseModified();
            }
        }

        public EditorSprite(Sprite sprite, EditorSpriteAtlas atlas) : base(atlas)
        {
            _id = sprite.Id;
            _x = sprite.X;
            _y = sprite.Y;
            _width = sprite.Width;
            _height = sprite.Height;
        }

        public Sprite ToSprite()
        {
            return new Sprite
            {
                Id = _id,
                X = _x,
                Y = _y,
                Width = _width,
                Height = _height,
            };
        }
    }

    public class EditorSpriteAtlas : EditorObject
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public ObservableCollection<EditorSprite> Sprites { get; set; }

        public Bitmap? CachedBmp { get; set; }

        public EditorSpriteAtlas(EditorDocument doc) : base(doc)
        {
            Id = "";
            Path = "";
            Sprites = new ObservableCollection<EditorSprite>();
        }

        public EditorSpriteAtlas(SpriteAtlas src, EditorDocument doc) : base(doc)
        {
            Id = src.Id;
            Path = src.Path;
            Sprites = new ObservableCollection<EditorSprite>();

            foreach (var sprite in src.Sprites)
            {
                Sprites.Add(new EditorSprite(sprite, this));
            }
        }

        public SpriteAtlas ToSpriteAtlas()
        {
            SpriteAtlas atlas = new SpriteAtlas();
            atlas.Id = Id;
            atlas.Path = Path;
            atlas.Sprites = new List<Sprite>();
            foreach (var sprite in Sprites)
            {
                atlas.Sprites.Add(sprite.ToSprite());
            }
            return atlas;
        }
    }
}