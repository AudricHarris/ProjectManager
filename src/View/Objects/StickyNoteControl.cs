// Avalonia
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;

// Model
using Model.Items;

namespace View.Objects
{
	/// <summary>
	/// Sticky note visual — drag to move, double-click or context-menu to edit.
	/// </summary>
	public class StickyNoteControl : Canvas
	{
		public static readonly StyledProperty<StickyNote> ItemProperty =
			AvaloniaProperty.Register<StickyNoteControl, StickyNote>(nameof(Item));

		public StickyNote Item
		{
			get => GetValue(ItemProperty);
			set => SetValue(ItemProperty, value);
		}

		private readonly TextBlock _textBlock;
		private readonly Border    _border;
		private          bool      _dragging;
		private          Point     _dragOffset;

		public StickyNoteControl()
		{
			Width  = 240;
			Height = 160;

			Effect = new DropShadowEffect
			{
				BlurRadius = 8,
				Color      = Colors.Black,
				OffsetX    = 2,
				OffsetY    = 4,
				Opacity    = 0.35
			};

			_border = new Border
			{
				Background      = new SolidColorBrush(Color.Parse("#FFF9C4")),
				BorderBrush     = new SolidColorBrush(Colors.Orange),
				BorderThickness = new Thickness(1.5),
				CornerRadius    = new CornerRadius(8),
				Padding         = new Thickness(12)
			};

			_textBlock = new TextBlock
			{
				TextWrapping      = TextWrapping.Wrap,
				FontSize          = 14,
				Foreground        = Brushes.Black,
				VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top
			};

			_border.Child = _textBlock;
			Children.Add(_border);

			this.GetObservable(ItemProperty).Subscribe(UpdateFromModel);
			Cursor = new Cursor(StandardCursorType.SizeAll);
			
			DoubleTapped += (sender, e) =>
			{
				OpenEditPopup();
				e.Handled = true;
			};
		}

		private void UpdateFromModel(StickyNote? note)
		{
			if (note == null) return;
			Canvas.SetLeft(this, note.Position.X);
			Canvas.SetTop(this, note.Position.Y);
			Width  = note.Width  > 0 ? note.Width  : 240;
			Height = note.Height > 0 ? note.Height : 160;
			_textBlock.Text = note.Text ?? "";
		}

		// ── Drag ─────────────────────────────────────────────────────────────────
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
				Item?.UpdatePos(new Point(x, y));
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


		public void OpenEditPopup()
		{
			if (Item == null) return;

			var popup = new Window
			{
				Title                 = "Edit Sticky Note",
				Width                 = 420,
				Height                = 320,
				Background            = new SolidColorBrush(Color.Parse("#2D2D3D")),
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			};

			var label = new TextBlock
			{
				Text       = "Edit note text:",
				Foreground = Brushes.LightGray,
				FontSize   = 14,
				Margin     = new Thickness(16, 12, 16, 4)
			};

			var textBox = new TextBox
			{
				Text          = Item.Text,
				AcceptsReturn = true,
				TextWrapping  = TextWrapping.Wrap,
				FontSize      = 15,
				Foreground    = Brushes.White,
				Background    = new SolidColorBrush(Color.Parse("#3D3D50")),
				Margin        = new Thickness(16, 0, 16, 12),
				Height        = 200
			};

			var save = new Button
			{
				Content             = "💾  Save",
				FontSize            = 15,
				FontWeight          = Avalonia.Media.FontWeight.SemiBold,
				Foreground          = Brushes.White,
				Background          = new SolidColorBrush(Color.Parse("#40a042")),
				Margin              = new Thickness(16, 0, 16, 16),
				Padding             = new Thickness(12, 8),
				CornerRadius        = new CornerRadius(8),
				HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
			};

			save.Click += (_, _) =>
			{
				Item.Text       = textBox.Text ?? "";
				_textBlock.Text = Item.Text;
				popup.Close();
			};

			var container = new StackPanel();
			container.Children.Add(label);
			container.Children.Add(textBox);
			container.Children.Add(save);
			popup.Content = container;
			popup.Show();
		}
	}
}
