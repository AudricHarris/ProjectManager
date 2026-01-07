public class Projet
{
	private static int nbProjet = 0;

	private int      id;
	private string   nom;
	private string   desc;
	private DateTime dateCreation;

	public Projet(string nom, string desc, DateTime dateCreation)
	{
		this.id = ++nbProjet;
		this.nom = nom;
		this.desc = desc;
		this.dateCreation = dateCreation;
	}

	public Projet(string nom, string desc) : this(nom, desc, DateTime.Now) {}
	public Projet(string nom) : this(nom, "") {}

	public int getId() { return this.id; }
	public string getNom() { return this.nom; }
	public string getDesc() { return this.desc; }
	public DateTime getDate() { return this.dateCreation; }
}
