using Model.Containers;
using Model.Items;
using Model;

/**
 * Controller :
 * Main link bitween view and Model
 * The instance that is first started by the app
 * Contains one attribtue project.
 */
public class Controller
{
	private Project? project;

	public Controller()	{ this.project = null; }

	//       SETTERS        //
	public void setProject(Project? p) { this.project = p; }

	//       GETTERS        //
	public Project? getProject() { return this.project; }

	//----------------------//
	//   Instance methods   //
	//----------------------//
	public void addItem(BoardItem item)
	{
		if (this.project != null)
			this.project.addItem(item);
	}

	public void saveProject()
	{
		if (this.project != null)
			ProjectRepository.saveProject(this.project);
	}

	override
	public String ToString()
	{
		
		if (this.project == null) return "";
		
		String res = this.project.ToString();

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

		ProjectRepository.deserialize();
		
		c.setProject(ProjectRepository.loadProject(0));
		Console.WriteLine(c);

		c.setProject(ProjectRepository.loadProject(1));
		Console.WriteLine(c);
	}

}
