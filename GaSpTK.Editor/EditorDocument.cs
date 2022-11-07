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
        public ObservableCollection<EditorAnimation> Animation { get; set; }

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
            Animation = new ObservableCollection<EditorAnimation>();
            Atlas = new ObservableCollection<EditorSpriteAtlas>();
            Event = new ObservableCollection<EventInfo>();
            Metadata = new ObservableCollection<MetaPropInfo>();

            foreach (var anim in file.Animation)
            {
                Animation.Add(new EditorAnimation(anim, this));
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
                f.Animation.Add(anim.ToSpriteAnim());
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

    public abstract class EditorTrack<TData> : EditorObject
    {
        public struct Keyframe
        {
            public int frame;
            public TData data;
        }

        public ObservableCollection<Keyframe> keyframes = new ObservableCollection<Keyframe>();

        public EditorTrack(EditorAnimation parent) : base(parent)
        {
            keyframes.CollectionChanged += (sender, e) =>
            {
                this.RaiseModified();
            };
        }

        public void Insert(int frame, TData data)
        {
            Keyframe newKeyframe = new Keyframe
            {
                frame = frame,
                data = data
            };

            for (int i = 0; i < keyframes.Count; i++)
            {
                if (keyframes[i].frame > frame)
                {
                    keyframes.Insert(i, newKeyframe);
                    break;
                }
                else if (keyframes[i].frame == frame)
                {
                    keyframes[i] = newKeyframe;
                    break;
                }
            }

            keyframes.Add(newKeyframe);
        }
    }

    public class EditorSpriteTrack : EditorTrack<EditorSpriteTrack.Data>
    {
        public struct Data
        {
            public string atlasId;
            public string spriteId;
        }

        public EditorSpriteTrack(EditorAnimation parent) : base(parent)
        {
        }
    }

    public class EditorAnimation : EditorObject
    {
        private string _id = "";

        public string Id
        {
            get => _id;
            set
            {
                this.RaiseAndSetIfChanged(ref _id, value);
                this.RaiseModified();
            }
        }

        public ObservableCollection<EditorSpriteTrack> SpriteTracks = new ObservableCollection<EditorSpriteTrack>();

        public EditorAnimation(EditorDocument doc) : base(doc)
        {
            SpriteTracks.CollectionChanged += (sender, e) =>
            {
                RaiseModified();
            };
        }

        public EditorAnimation(SpriteAnim anim, EditorDocument doc) : base(doc)
        {
            _id = anim.Id;
        }

        public SpriteAnim ToSpriteAnim()
        {
            var anim = new SpriteAnim();
            anim.Id = _id;

            return anim;
        }
    }
}