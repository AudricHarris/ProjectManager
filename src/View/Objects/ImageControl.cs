using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Input;

using Model.Items;

namespace View.Objects
{
	/// <summary>
	/// Renders an ImageObject on the canvas with drag-to-move.
	/// </summary>
	public class ImageControl : Canvas
	{
		public ImageObject Item { get; private set; }

		private bool  _dragging;
		private Point _dragOffset;

		public ImageControl(ImageObject item)
		{
			Item   = item;
			Width  = item.Width  > 0 ? item.Width  : 240;
			Height = item.Height > 0 ? item.Height : 180;

			Effect = new DropShadowEffect
			{
				BlurRadius = 8,
				Color      = Colors.Black,
				OffsetX    = 2,
				OffsetY    = 4,
				Opacity    = 0.35
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
				Background      = new SolidColorBrush(Color.Parse("#1A1A2A")),
				BorderBrush     = new SolidColorBrush(Color.Parse("#555577")),
				BorderThickness = new Thickness(1.5),
				CornerRadius    = new CornerRadius(8),
				Padding         = new Thickness(4)
			};

			var stack = new StackPanel { Spacing = 4 };

			// Try to load image
			try
			{
				if (File.Exists(Item.Path))
				{
					var bmp = new Bitmap(Item.Path);
					var img = new Avalonia.Controls.Image
					{
						Source  = bmp,
						Width   = Width - 8,
						Height  = Height - 30,
						Stretch = Stretch.Uniform
					};
					stack.Children.Add(img);
				}
				else
				{
					stack.Children.Add(new TextBlock
					{
						Text       = "🖼️  Image not found",
						Foreground = Brushes.OrangeRed,
						FontSize   = 13,
						Margin     = new Thickness(8)
					});
				}
			}
			catch
			{
				stack.Children.Add(new TextBlock
				{
					Text       = "🖼️  Cannot load image",
					Foreground = Brushes.OrangeRed,
					FontSize   = 13,
					Margin     = new Thickness(8)
				});
			}

			// Filename label
			stack.Children.Add(new TextBlock
			{
				Text       = Item.Name,
				Foreground = Brushes.LightGray,
				FontSize   = 11,
				TextTrimming = TextTrimming.CharacterEllipsis,
				MaxWidth   = Width - 16,
				Margin     = new Thickness(4, 0, 4, 4)
			});

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
