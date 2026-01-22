using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

using View.Systems;

namespace View.Panels
{
	public class TopPanel : StackPanel
	{
		public TopPanel()
		{
			Orientation = Orientation.Horizontal;
			HorizontalAlignment = HorizontalAlignment.Stretch;
			Height = 60;
			
			TextBlock title = SystemStyle.GenerateTitle("Project Manager");

			Children.Add(title);
		}
	}
}

