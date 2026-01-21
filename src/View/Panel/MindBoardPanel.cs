using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;

using Model.Containers;
using View.Systems;

namespace View.Panel
{
	public class MindBoardPanel : DockPanel
	{
	    private MouseManager mm;
	    private Project p;

		public MindBoardPanel(Project p, MouseManager mm )
		{
            this.mm = mm;
			this.p = p;

        }

		public void NewProject_OnClick(object? sender, RoutedEventArgs e)
        {
    	        mm.NewProjectEvent();
        }

	}
}
