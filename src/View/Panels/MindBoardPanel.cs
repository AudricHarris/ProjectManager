using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;

using Model.Containers;
using View.Systems;

namespace View.Panels
{
	public class MindBoardPanel : DockPanel
	{
	    private MouseManager mm;
	    private Project p;

		public MindBoardPanel(Project p, MouseManager mm )
		{
            this.mm = mm;
			this.p = p;
			
			TextBlock text = SystemStyle.GenerateTitle(p.GetName());
			Canvas canva = new Canvas();
			
			this.Background = SystemStyle.Background;

			DockPanel.SetDock(text, Dock.Top);
			this.Children.Add(text);
			this.Children.Add(canva);
        }

		public void NewProject_OnClick(object? sender, RoutedEventArgs e)
        {
    	        mm.NewProjectEvent();
        }

	}
}
