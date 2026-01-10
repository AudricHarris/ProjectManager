using Model.Containers;
using Model.Items;
using Model;

public class Controller
{
	private Project? project;

	public Controller()
	{
		this.project = null;
	}
	
	public void setProject(Project? p) { this.project = p; }

	public Project? getProject() { return this.project; }

	public void addNote(string text)
	{
		if (this.project != null)
			this.project.addItem(new StickyNote(text));
	}

	public void addImage(string text)
	{
		if (this.project != null)
			this.project.addItem(new Image(text, "~/Documents/"));
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
