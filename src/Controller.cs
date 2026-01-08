using Model.Containers;
using Model.Items;

public class Controller
{
	private Project project;

	public Controller()
	{
		this.project = new Project("Mattfula .Feat norman", "La collab du sciecle entre deux personne qui aime un peu trop les enfants");
	}

	public void addNote(string text)
	{
		this.project.addItem(new StickyNote(text));
	}

	override
	public String ToString()
	{
		String res = "Controller : \n\n" + this.project.ToString();

		return res;
	}


	public static void Main(String[] args)
	{
		Controller c = new Controller();
		c.addNote("test.png");
		c.addNote("youtube.com");
		c.addNote("trouver des enfants");
		c.addNote("trouver des youtubeur minecraft");
		Console.WriteLine(c);
	}

}
