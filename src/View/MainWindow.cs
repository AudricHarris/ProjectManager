using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

using View.Panel;

namespace View
{
	public class MainWindow : Window
	{
		public MainWindow()
		{
			Title = "Project Manager";
			Width = 800;
			Height = 600;

			DockPanel mainPanel = new DockPanel();
			TopPanel tp = new TopPanel();
			DockPanel.SetDock(tp, Dock.Top);
			mainPanel.Children.Add(tp);

			Content = mainPanel;
		}
	}
}
