using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Layout;

namespace View.Systems
{
	public class SystemStyle
	{
		public static readonly SolidColorBrush Background = new SolidColorBrush(Color.FromRgb(63, 63, 70)); // Light : 237, 242, 247  Dark : 63, 63, 70
		public static readonly SolidColorBrush TopBanner = new SolidColorBrush(Color.FromRgb(45, 45, 48)); //  Light :   0, 120, 212  Dark : 45, 45, 48

		public static TextBlock GenerateTitle(String title)
		{
			TextBlock titleBlock = new TextBlock
			{
				Text = title,
				FontSize = 30,
				Foreground = Brushes.White,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(0, 20, 0, 30)
            };

			return titleBlock;
		}

		public static TextBlock GenerateText(String text)
		{
			TextBlock textBlock = new TextBlock
			{
				Text = text,
				Foreground = Brushes.White
			};

			return textBlock;
		}
	}
}
