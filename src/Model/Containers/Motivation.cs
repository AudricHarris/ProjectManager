using Model.Items;

namespace Model.Containers
{
	public class Motivation
	{
		private static List<Motivation> lstItems = new List<Motivation>();

		private int        id;
		private string     name;
		private BoardItem? content;

		public Motivation(int id, string nom, BoardItem? item)
		{
			this.id = id;
			this.name = nom;
			this.content = item;
		}


		public static void chargerMotivation(){}

		public static Motivation randomMotivation() 
		{
			return new Motivation(1,"", null);
		}
	}
}
