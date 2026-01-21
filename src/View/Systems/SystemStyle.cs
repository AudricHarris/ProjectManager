using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Layout;

namespace View.Systems
{
	public class SystemStyle
	{
		public static TextBlock GenerateTitle(String title)
		{
			TextBlock titleBlock = new TextBlock
			{
				Text = title,
				FontSize = 30,
				Foreground = Brushes.Black,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(0, 20, 0, 30)
            };

			return titleBlock;
		}

		public static TextBlock GenerateText(String text)
		{
			TextBlock textBlock = new TextBlock
			{
				Text = text
			};

			return textBlock;
		}
	}
}
