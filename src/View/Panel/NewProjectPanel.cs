using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;

using View.Systems;

namespace View.Panel
{
	public class NewProjectPanel : StackPanel
	{
	    private MouseManager mm;
	    private TextBox title;
	    private TextBox desc;

		public NewProjectPanel(MouseManager mm)
		{
            this.mm = mm;

			StackPanel menu = new StackPanel
			{
				Orientation = Orientation.Vertical,
				Spacing = 20,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			// Title
			menu.Children.Add(SystemStyle.GenerateTitle("Create Project"));


            menu.Children.Add(SystemStyle.GenerateText("Project Name"));
            this.title = new TextBox{Width = 300, Height = 40, HorizontalAlignment = HorizontalAlignment.Left, FontSize = 20};
            menu.Children.Add(this.title);

            menu.Children.Add(SystemStyle.GenerateText("Project Description"));
            this.desc = new TextBox{Width = 500, Height = 200, FontSize = 20, AcceptsReturn = true, TextWrapping = TextWrapping.Wrap};
            menu.Children.Add(this.desc);

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

            newProject.Click += this.UpdateProject_OnClick;

            menu.Children.Add(newProject);
            Children.Add(menu);
        }

		public void UpdateProject_OnClick(object? sender, RoutedEventArgs e)
        {
    	        mm.CreateUpdateProject(null, this.title.Text, this.desc.Text );
        }
	}
}
