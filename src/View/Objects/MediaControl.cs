using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;

using Model.Items;

namespace View.Objects
{
	/// <summary>
	/// Renders an audio file card on the canvas.
	/// Drag to move; the card shows the filename and a play note.
	/// (Full audio playback requires LibVLCSharp or NAudio — wired up via MediaPlayerHelper.)
	/// </summary>
	public class AudioControl : Canvas
	{
		public AudioItem Item { get; private set; }

		private bool  _dragging;
		private Point _dragOffset;

		public AudioControl(AudioItem item)
		{
			Item   = item;
			Width  = 300;
			Height = 80;

			Effect = new DropShadowEffect
			{
				BlurRadius = 6,
				Color      = Colors.Black,
				OffsetX    = 2,
				OffsetY    = 3,
				Opacity    = 0.3
			};

			Canvas.SetLeft(this, item.Position.X);
			Canvas.SetTop(this, item.Position.Y);

			Build();
			Cursor = new Cursor(StandardCursorType.SizeAll);
		}

		private void Build()
		{
			var border = new Border
			{
				Background      = new SolidColorBrush(Color.Parse("#1E2A1E")),
				BorderBrush     = new SolidColorBrush(Color.Parse("#44AA66")),
				BorderThickness = new Thickness(1.5),
				CornerRadius    = new CornerRadius(10),
				Padding         = new Thickness(12, 10)
			};

			var row = new StackPanel
			{
				Orientation = Avalonia.Layout.Orientation.Horizontal,
				Spacing     = 12,
				VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
			};

			row.Children.Add(new TextBlock
			{
				Text     = "🎵",
				FontSize = 26,
				VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
			});

			var info = new StackPanel { Spacing = 3 };
			info.Children.Add(new TextBlock
			{
				Text       = Item.Name,
				Foreground = Brushes.White,
				FontWeight = FontWeight.SemiBold,
				FontSize   = 14,
				TextTrimming = TextTrimming.CharacterEllipsis,
				MaxWidth   = 200
			});
			info.Children.Add(new TextBlock
			{
				Text       = "Audio file",
				Foreground = new SolidColorBrush(Color.Parse("#66CC88")),
				FontSize   = 11
			});

			row.Children.Add(info);
			border.Child = row;
			Children.Add(border);
		}

		protected override void OnPointerPressed(PointerPressedEventArgs e)
		{
			base.OnPointerPressed(e);
			if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
			{
				_dragging   = true;
				_dragOffset = e.GetPosition(this);
				e.Pointer.Capture(this);
				e.Handled = true;
			}
		}

		protected override void OnPointerMoved(PointerEventArgs e)
		{
			base.OnPointerMoved(e);
			if (_dragging && Parent is Canvas canvas)
			{
				var pos  = e.GetPosition(canvas);
				double x = pos.X - _dragOffset.X;
				double y = pos.Y - _dragOffset.Y;
				Canvas.SetLeft(this, x);
				Canvas.SetTop(this, y);
				Item.UpdatePos(new Point(x, y));
				e.Handled = true;
			}
		}

		protected override void OnPointerReleased(PointerReleasedEventArgs e)
		{
			base.OnPointerReleased(e);
			if (_dragging) { _dragging = false; e.Pointer.Capture(null); }
		}
	}

	/// <summary>
	/// Renders a video file card on the canvas.
	/// </summary>
	public class VideoControl : Canvas
	{
		public VideoItem Item { get; private set; }

		private bool  _dragging;
		private Point _dragOffset;

		public VideoControl(VideoItem item)
		{
			Item   = item;
			Width  = 340;
			Height = 90;

			Effect = new DropShadowEffect
			{
				BlurRadius = 6,
				Color      = Colors.Black,
				OffsetX    = 2,
				OffsetY    = 3,
				Opacity    = 0.3
			};

			Canvas.SetLeft(this, item.Position.X);
			Canvas.SetTop(this, item.Position.Y);

			Build();
			Cursor = new Cursor(StandardCursorType.SizeAll);
		}

		private void Build()
		{
			var border = new Border
			{
				Background      = new SolidColorBrush(Color.Parse("#1A1A2E")),
				BorderBrush     = new SolidColorBrush(Color.Parse("#6644CC")),
				BorderThickness = new Thickness(1.5),
				CornerRadius    = new CornerRadius(10),
				Padding         = new Thickness(12, 10)
			};

			var row = new StackPanel
			{
				Orientation       = Avalonia.Layout.Orientation.Horizontal,
				Spacing           = 12,
				VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
			};

			row.Children.Add(new TextBlock
			{
				Text     = "🎬",
				FontSize = 26,
				VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
			});

			var info = new StackPanel { Spacing = 3 };
			info.Children.Add(new TextBlock
			{
				Text         = Item.Name,
				Foreground   = Brushes.White,
				FontWeight   = FontWeight.SemiBold,
				FontSize     = 14,
				TextTrimming = TextTrimming.CharacterEllipsis,
				MaxWidth     = 240
			});
			info.Children.Add(new TextBlock
			{
				Text       = "Video file",
				Foreground = new SolidColorBrush(Color.Parse("#9977EE")),
				FontSize   = 11
			});

			row.Children.Add(info);
			border.Child = row;
			Children.Add(border);
		}

		protected override void OnPointerPressed(PointerPressedEventArgs e)
		{
			base.OnPointerPressed(e);
			if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
			{
				_dragging   = true;
				_dragOffset = e.GetPosition(this);
				e.Pointer.Capture(this);
				e.Handled = true;
			}
		}

		protected override void OnPointerMoved(PointerEventArgs e)
		{
			base.OnPointerMoved(e);
			if (_dragging && Parent is Canvas canvas)
			{
				var pos  = e.GetPosition(canvas);
				double x = pos.X - _dragOffset.X;
				double y = pos.Y - _dragOffset.Y;
				Canvas.SetLeft(this, x);
				Canvas.SetTop(this, y);
				Item.UpdatePos(new Point(x, y));
				e.Handled = true;
			}
		}

		protected override void OnPointerReleased(PointerReleasedEventArgs e)
		{
			base.OnPointerReleased(e);
			if (_dragging) { _dragging = false; e.Pointer.Capture(null); }
		}
	}
}
