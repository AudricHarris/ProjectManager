using Model.Containers;
using Model.Items;
using Model;
using View;
using Avalonia;

/**
 * Controller :
 * Main link bitween view and Model
 * The instance that is first started by the app
 * Contains one attribtue project.
 */
public class Controller
{
	private Project? _project;

	public Controller()	{ this._project = null; }

	//       SETTERS        //
	public void SetProject(Project? p) { this._project = p; }

	//       GETTERS        //
	public Project? GetProject() { return this._project; }
	public List<Project> getListProject() { return ProjectRepository.GetLstProject();}

	//----------------------//
	//   Instance methods   //
	//----------------------//
	public void AddItem(BoardItem item)
	{
		if (this._project != null)
			this._project.AddItem(item);
	}

	public void SaveProject()
	{
		if (this._project != null)
		{
			ProjectRepository.SaveProject(this._project);
			ProjectRepository.Serialize();
		}
	}

	override
	public String ToString()
	{
		
		if (this._project == null) return "";
		
		String res = this._project.ToString();

		return res;
	}

	//----------------------//
	//    Static Methods    //
	//----------------------//
	public static void Main(String[] args)
	{
		Console.BackgroundColor = ConsoleColor.DarkBlue;
		Console.WriteLine("\nController : \n");
		Console.ResetColor();

		Controller c = new Controller();

		ProjectRepository.Deserialize();
		
		foreach(Project p in c.getListProject())
			Console.WriteLine(p);

		MainWindow.SCtrl = c;
		Controller.BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
	}
	
	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<Init>()
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace();
}
