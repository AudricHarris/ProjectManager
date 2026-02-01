
using View.Panels;
using Model.Containers;
using Model.Items;

namespace View.Systems
{
	public class MouseManager
	{
		public MainWindow MainWindow { get; set; }

		public event Action<BoardItem>? ItemAdded;

		public MouseManager(MainWindow window)
		{
			this.MainWindow = window;
		}

		public void NewProjectEvent(Project? p)
		{
			this.MainWindow.SwitchPanel(new NewProjectPanel(this, p));
		}

		public void CreateUpdateProject(Project? p, string name, string desc)
		{
			if (p == null && MainWindow.SCtrl != null)
			{
				MainWindow.SCtrl.SetProject(new Project(name, desc));
				MainWindow.SCtrl.SaveProject();
				this.MainWindow.SwitchPanel(new MenuPanel(this.MainWindow, MainWindow.SCtrl.getListProject(), this));
			}
			else if (MainWindow.SCtrl != null)
			{
				p.SetDesc(desc);
				p.SetName(name);
				MainWindow.SCtrl.SetProject(p);
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

		public void NotifyNewItemCreated(BoardItem newItem)
		{
			ItemAdded?.Invoke(newItem);
		}

	}
}
