
using View.Panels;

using Model.Containers;

namespace View.Systems
{
	public class MouseManager
	{
		public MainWindow MainWindow {get; set;}

		public MouseManager(MainWindow window)
		{
			this.MainWindow = window;
		}
		
        public void NewProjectEvent()
        {
	        this.MainWindow.SwitchPanel(new NewProjectPanel(this));
        }

        public void CreateUpdateProject(Project? p, String name, String desc)
        {
			if ( p == null && MainWindow.SCtrl != null)
			{
				MainWindow.SCtrl.SetProject(new Project(name, desc));
				MainWindow.SCtrl.SaveProject();
				this.MainWindow.SwitchPanel(new MenuPanel(this.MainWindow, MainWindow.SCtrl.getListProject(), this));
			}
        }

        public Project? ProjectOpenEvent(Project? p)
        {
        	Console.WriteLine(p);
        	if (p != null)
        		this.MainWindow.SwitchPanel(new MindBoardPanel(p, this));
        	return p;
        }

        public void CancelProject()
        {
			this.MainWindow.SwitchPanel(new MenuPanel(this.MainWindow, MainWindow.SCtrl.getListProject(), this));
        }

        public void DeleteProject(Project p)
        {
        	MainWindow.SCtrl.SetProject(p);
        	MainWindow.SCtrl.DeleteProject();
			this.MainWindow.SwitchPanel(new MenuPanel(this.MainWindow, MainWindow.SCtrl.getListProject(), this));
        }
	}
}
