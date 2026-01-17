using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;


namespace View.Panel
{
	public class NewProjectPanel : StackPanel
	{
		public NewProjectPanel()
		{

			StackPanel menu = new StackPanel
			{
				Orientation = Orientation.Vertical,
				Spacing = 20,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			// Title
			menu.Children.Add(new TextBlock
			{
				Text = "Test :",
				FontSize = 30,
				Foreground = Brushes.Black,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(0, 20, 0, 30)
            });

			StackPanel projectsContainer = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Spacing = 15,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(40, 0, 40, 20)
            };

            Button newProject = new Button
            {
            	Content = "Create",
            	FontWeight = FontWeight.Bold,
            	FontSize = 20,
            	Foreground = Brushes.LightGray,
                Width = 200,
                Height = 100,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                CornerRadius = new CornerRadius(12),
                Background = new SolidColorBrush(Color.Parse("#40a042")),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 5, 0, 5)
            };

            menu.Children.Add(projectsContainer);
            menu.Children.Add(newProject);
            Children.Add(menu);
        }
	}
}
