//Avalonia
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Input;

//Model
using Model.Items;

namespace View.Objects
{
	public class StickyNoteControl : Canvas
	{
		public static readonly StyledProperty<StickyNote> ItemProperty =
			AvaloniaProperty.Register<StickyNoteControl, StickyNote>(nameof(Item));

		public StickyNote Item
		{
			get => GetValue(ItemProperty);
			set => SetValue(ItemProperty, value);
		}

		private TextBlock _textBlock;
		private Border _border;

		public StickyNoteControl()
		{
			Width = 240;
			Height = 160;

			Effect = new DropShadowEffect
			{
				BlurRadius = 8,
				Color = Colors.Black,
				OffsetX = 2,
				OffsetY = 4,
				Opacity = 0.35
			};

			_border = new Border
			{
				Background = new SolidColorBrush(Color.Parse("#FFF9C4")),
				BorderBrush = new SolidColorBrush(Colors.Orange),
				BorderThickness = new Thickness(1.5),
				CornerRadius = new CornerRadius(8),
				Padding = new Thickness(12)
			};

			_textBlock = new TextBlock
			{
				TextWrapping = TextWrapping.Wrap,
				FontSize = 14,
				Foreground = Brushes.Black,
				VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top
			};

			_border.Child = _textBlock;
			Children.Add(_border);

			this.GetObservable(ItemProperty).Subscribe(UpdateFromModel);
		}

		private void UpdateFromModel(StickyNote? note)
		{
			if (note == null) return;

			Canvas.SetLeft(this, note.Position.X);
			Canvas.SetTop(this, note.Position.Y);

			Width = note.Width > 0 ? note.Width : 240;
			Height = note.Height > 0 ? note.Height : 160;

			_textBlock.Text = note.Text ?? "";
		}

		protected override void OnPointerPressed(PointerPressedEventArgs e)
		{
			base.OnPointerPressed(e);
		}
	}
}
