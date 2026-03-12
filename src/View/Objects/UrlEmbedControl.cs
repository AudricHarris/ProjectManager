using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;

using Model.Items;

namespace View.Objects
{
	/// <summary>
	/// Renders a URL embed card on the canvas. Shows YouTube info or a generic link card.
	/// Supports drag to move.
	/// </summary>
	public class UrlEmbedControl : Canvas
	{
		public UrlEmbed Item { get; private set; }

		private bool  _dragging;
		private Point _dragOffset;

		public UrlEmbedControl(UrlEmbed item)
		{
			Item   = item;
			Width  = 320;
			Height = item.IsYouTube ? 200 : 100;

			Effect = new DropShadowEffect
			{
				BlurRadius = 8,
				Color      = Colors.Black,
				OffsetX    = 2,
				OffsetY    = 4,
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
				Background      = new SolidColorBrush(Color.Parse("#1E1E2E")),
				BorderBrush     = new SolidColorBrush(Color.Parse("#5555AA")),
				BorderThickness = new Thickness(1.5),
				CornerRadius    = new CornerRadius(10),
				Padding         = new Thickness(14)
			};

			var stack = new StackPanel { Spacing = 8 };

			// Icon + title row
			var header = new StackPanel
			{
				Orientation = Avalonia.Layout.Orientation.Horizontal,
				Spacing     = 8
			};

			string icon = Item.IsYouTube ? "▶️" : "🔗";
			header.Children.Add(new TextBlock
			{
				Text      = icon,
				FontSize  = 20,
				VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
			});
			header.Children.Add(new TextBlock
			{
				Text         = Item.IsYouTube ? "YouTube Video" : "Link",
				Foreground   = new SolidColorBrush(Color.Parse("#AAAAFF")),
				FontSize     = 13,
				FontWeight   = FontWeight.SemiBold,
				VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
			});

			stack.Children.Add(header);

			// Title
			stack.Children.Add(new TextBlock
			{
				Text         = Item.Title,
				Foreground   = Brushes.White,
				FontSize     = 14,
				FontWeight   = FontWeight.Bold,
				TextWrapping = TextWrapping.Wrap,
				MaxWidth     = 280
			});

			// URL (truncated)
			var urlText = Item.Url.Length > 50 ? Item.Url.Substring(0, 47) + "…" : Item.Url;
			stack.Children.Add(new TextBlock
			{
				Text       = urlText,
				Foreground = new SolidColorBrush(Color.Parse("#777799")),
				FontSize   = 11,
				TextWrapping = TextWrapping.NoWrap
			});

			if (Item.IsYouTube && Item.YouTubeEmbedId != null)
			{
				var thumbNote = new TextBlock
				{
					Text       = $"🎬  youtu.be/{Item.YouTubeEmbedId}",
					Foreground = new SolidColorBrush(Color.Parse("#FF4444")),
					FontSize   = 12,
					Margin     = new Thickness(0, 4, 0, 0)
				};
				stack.Children.Add(thumbNote);
			}

			border.Child = stack;
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
			if (_dragging)
			{
				_dragging = false;
				e.Pointer.Capture(null);
			}
		}
	}
}
