using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;


namespace View.Panels
{
	public class TopPanel : StackPanel
	{
		public TopPanel()
		{
			Background = Brushes.RoyalBlue;
			Orientation = Orientation.Horizontal;
			HorizontalAlignment = HorizontalAlignment.Stretch;
			Height = 60;
			
			TextBlock title = new TextBlock();

			title.Text = "Project Manager";
			title.FontSize = 24;
			title.FontStyle = FontStyle.Oblique;
			title.VerticalAlignment = VerticalAlignment.Center;
			title.Margin = new Thickness(20);

			Children.Add(title);
		}
	}
}

