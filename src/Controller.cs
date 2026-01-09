using Model.Containers;
using Model.Items;
using Model;

public class Controller
{
	private Project project;

	public Controller()
	{
		this.project = new Project("Mattfula .Feat norman", "La collab du sciecle entre deux personne qui aime un peu trop les enfants");
	}
	
	public void setProject(Project p) { this.project = p; }

	public Project getProject() { return this.project; }

	public void addNote(string text)
	{
		this.project.addItem(new StickyNote(text));
	}

	public void addImage(string text)
	{
		this.project.addItem(new Image(text, "~/Documents/"));
	}
	public void saveProject()
	{
		ProjectRepository.saveProject(this.project);
	}

	override
	public String ToString()
	{
		String res = this.project.ToString();

		return res;
	}


	public static void Main(String[] args)
	{
		Console.BackgroundColor = ConsoleColor.DarkBlue;
		Console.WriteLine("\nController : \n");
		Console.ResetColor();

		Controller c = new Controller();
		c.addImage("test.png");
		c.addNote("youtube.com");
		c.addNote("trouver des enfants");
		c.addNote("trouver des youtubeur minecraft");

		Console.WriteLine(c);

		c.saveProject();
		c.setProject(new Project("Case prison", "Le feat avec norman a fait le bad buzz"));
		c.addNote("EchaperPrison.mp3");
		c.addImage("MattEnPrison.jpeg");

		Console.WriteLine(c);

		Console.BackgroundColor = ConsoleColor.DarkBlue;
		Console.WriteLine("\nSerialize : \n");
		Console.ResetColor();

		c.saveProject();

		ProjectRepository.serialize();

	}

}
