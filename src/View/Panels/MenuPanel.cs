using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;

using Model.Containers;
using View.Systems;

namespace View.Panels
{
	public class MenuPanel : StackPanel
	{
	    private MouseManager mm;

		public MenuPanel(MainWindow window, List<Project> lstProjects, MouseManager mm )
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
			menu.Children.Add(new TextBlock
			{
				Text = "List Project :",
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

            foreach (Project p in lstProjects)
            {
                Button projectButton = new Button
                {
                    Width = 800,
                    Height = 100,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    CornerRadius = new CornerRadius(12),
                    Background = new SolidColorBrush(Color.Parse("#2D2D3D")), // Dark modern background
                    Padding = new Thickness(20),
                    Margin = new Thickness(0, 5, 0, 5)
                };

                // Grid inside button to organize content: left info + right date
                Grid contentGrid = new Grid();
                contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Left side: Name and Description
                StackPanel leftInfo = new StackPanel
                {
                    Spacing = 6,
                    VerticalAlignment = VerticalAlignment.Center
                };

                leftInfo.Children.Add(new TextBlock
                {
                    Text = p.Name,
                    FontSize = 20,
                    FontWeight = FontWeight.Bold,
                    Foreground = Brushes.White
                });

                leftInfo.Children.Add(new TextBlock
                {
                    Text = p.Desc,
                    FontSize = 14,
                    Foreground = Brushes.LightGray,
                    TextWrapping = TextWrapping.Wrap
                });

                Grid.SetColumn(leftInfo, 0);

                // Right side: Date
                TextBlock dateBlock = new TextBlock
                {
                    Text = p.DateCreation.ToString("dd MMM yyyy"),
                    FontSize = 14,
                    Foreground = Brushes.Cyan,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(20, 0, 0, 0)
                };

                Grid.SetColumn(dateBlock, 2);

                contentGrid.Children.Add(leftInfo);
                contentGrid.Children.Add(dateBlock);

                projectButton.Content = contentGrid;

                // Optional: hover effect
                projectButton.Classes.Add("projectCard"); // You can define styles later if you want

                projectsContainer.Children.Add(projectButton);
                projectButton.Tag = p;
                projectButton.Click += this.ProjectOpened_OnClick;
            }
            
            Button newProject = new Button
            {
            	Content = "New Project",
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

            newProject.Click += this.NewProject_OnClick;

            menu.Children.Add(projectsContainer);
            menu.Children.Add(newProject);
            Children.Add(menu);


        }

		public void NewProject_OnClick(object? sender, RoutedEventArgs e)
        {
    	        mm.NewProjectEvent();
        }

        public void ProjectOpened_OnClick(object? sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
    	        mm.ProjectOpenEvent((Project?)b.Tag);
    	    }
        }

	}
}
