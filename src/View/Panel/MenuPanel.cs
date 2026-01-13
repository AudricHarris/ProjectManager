using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace View.Panel
{
	public class MenuPanel : StackPanel
	{
		public MenuPanel()
		{
			Background = Brushes.DarkSlateGray;

			StackPanel menu = new StackPanel
			{
				Orientation = Orientation.Vertical,
				Spacing = 20,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			Grid outerGrid = new Grid();
			outerGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
			outerGrid.VerticalAlignment = VerticalAlignment.Stretch;

			var buttonPanel = new StackPanel
			{
				Orientation = Orientation.Vertical,
				Spacing = 20,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			Button p1 = new Button { Content = "Norman", Width = 150, Height = 50 };
			Button p2 = new Button { Content = "Matt", Width = 150, Height = 50 };
			Button p3 = new Button { Content = "WQLF", Width = 150, Height = 50 };

			buttonPanel.Children.Add(p1);
			buttonPanel.Children.Add(p2);
			buttonPanel.Children.Add(p3);

			menu.Children.Add(new TextBlock{Text="List Project : ", FontSize = 30});

			outerGrid.Children.Add(buttonPanel);
			menu.Children.Add(outerGrid);

			Children.Add(menu);
		}
	}
}
