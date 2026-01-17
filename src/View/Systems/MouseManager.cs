using Avalonia.Interactivity;

using View.Panel;

namespace View.Systems
{
	public class MouseManager
	{
		public MainWindow MainWindow {get; set;}

		public MouseManager(MainWindow window)
		{
			this.MainWindow = window;
		}
		
		public void NewProject_OnClick(object? sender, RoutedEventArgs e)
        {
    	        Console.WriteLine("Clicked in MouseManager");
    	        this.MainWindow.SwitchPanel(new NewProjectPanel());
        }
	}
}
