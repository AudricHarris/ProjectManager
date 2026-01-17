using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;

using Avalonia.Markup.Xaml;

using View.Panel;
using Model.Containers;
using View.Systems;

namespace View
{
	public class MainWindow : Window
	{
		public static Controller? SCtrl {get;set;}
		
		public StackPanel Panel {get; set;}

		public MainWindow()
		{
			AvaloniaXamlLoader.Load(this);	

			Title = "Project Manager";
			Width = 800;
			Height = 600;
			// Creation of panels
			MouseManager mm = new MouseManager(this);

			DockPanel mainPanel = new DockPanel();
			TopPanel tp = new TopPanel();
			MenuPanel mp;
			if (SCtrl != null)
				mp = new MenuPanel(this, SCtrl.getListProject(),mm);
			else
				mp = new MenuPanel(this, new List<Project>(), mm);

			this.Panel = mp;
			// Placement
			DockPanel.SetDock(tp, Dock.Top);
			mainPanel.Children.Add(tp);
			mainPanel.Children.Add(this.Panel);
			Content = mainPanel;

		}

		public void SwitchPanel(StackPanel panel)
		{
			DockPanel? mainPanel = (DockPanel?) Content;
			
			if (mainPanel != null)
				mainPanel.Children.Remove(this.Panel);

			this.Panel = panel;
			if (mainPanel != null)
				mainPanel.Children.Add(this.Panel);
			
			this.InvalidateVisual();
		}
	}
}
