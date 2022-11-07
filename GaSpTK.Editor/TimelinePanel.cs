using Avalonia.Controls;
using Avalonia.Media;

namespace GaSpTK.Editor
{
    public class TimelinePanel : Panel
    {
        public static readonly Avalonia.StyledProperty<double> TimeScaleProperty = Avalonia.AvaloniaProperty.Register<TimelinePanel, double>(nameof(TimeScale), 8.0);
        public static readonly Avalonia.StyledProperty<IBrush> FillProperty = Avalonia.AvaloniaProperty.Register<TimelinePanel, IBrush>(nameof(Fill));
        public static readonly Avalonia.StyledProperty<double> LineIntervalProperty = Avalonia.AvaloniaProperty.Register<TimelinePanel, double>(nameof(LineInterval), 4.0);
        public static readonly Avalonia.DirectProperty<TimelinePanel, double> PlayHeadProperty = Avalonia.AvaloniaProperty.RegisterDirect<TimelinePanel, double>(
            nameof(PlayHead),
            o => o.PlayHead,
            (o, v) => o.PlayHead = v,
            default(double),
            Avalonia.Data.BindingMode.TwoWay);

        public static readonly Avalonia.DirectProperty<TimelinePanel, double> MaxTimeProperty = Avalonia.AvaloniaProperty.RegisterDirect<TimelinePanel, double>(
            nameof(MaxTime),
            o => o.MaxTime,
            (o, v) => o.MaxTime = v);

        public IBrush Fill
        {
            get => GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public double LineInterval
        {
            get => GetValue(LineIntervalProperty);
            set => SetValue(LineIntervalProperty, value);
        }

        public double TimeScale
        {
            get => GetValue(TimeScaleProperty);
            set => SetValue(TimeScaleProperty, value);
        }

        public double PlayHead
        {
            get => _playHead;
            set
            {
                if (value < 0.0) value = 0.0;
                if (value >= _maxTime) value = _maxTime;

                if (SetAndRaise(PlayHeadProperty, ref _playHead, value))
                {
                    this.InvalidateVisual();
                }
            }
        }

        public double MaxTime
        {
            get => _maxTime;
            set
            {
                if (SetAndRaise(MaxTimeProperty, ref _maxTime, value))
                {
                    this.InvalidateVisual();
                }
            }
        }

        private double _playHead = 0.0;
        private double _maxTime = 0.0;
        private Pen lineIntervalPen = new Pen(Brushes.White, 1.0);
        private Pen playHeadPen = new Pen(Brushes.White, 2.0);
        private IBrush maskBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0), 0.5);

        public TimelinePanel()
        {
            this.PointerPressed += (sender, args) =>
            {
                args.Handled = true;
                args.Pointer.Capture(this);
                PlayHead = args.GetPosition(this).X / TimeScale;
            };

            this.PointerMoved += (sender, args) =>
            {
                if (args.Pointer.Captured == this)
                {
                    args.Handled = true;
                    PlayHead = args.GetPosition(this).X / TimeScale;
                }
            };
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            
            lineIntervalPen.Brush = Fill;

            if (LineInterval > 0.0)
            {
                for (var x = 0.0; x < Bounds.Width; x += LineInterval * TimeScale)
                {
                    context.DrawLine(lineIntervalPen, new Avalonia.Point(x, 0), new Avalonia.Point(x, Bounds.Height));
                }
            }

            double playT = System.Math.Floor(PlayHead) * TimeScale;
            context.DrawLine(playHeadPen, new Avalonia.Point(playT, 0), new Avalonia.Point(playT, Bounds.Height));

            double maxT = System.Math.Floor(MaxTime) * TimeScale;
            context.DrawRectangle(maskBrush, null, new Avalonia.Rect(maxT, 0, Bounds.Width - maxT, Bounds.Height));
        }
    }
}