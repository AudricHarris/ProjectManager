using Model.Items;

namespace Model.Containers
{
	/**
	 * Motivation :
	 * Motivation are a Board item
	 * Items can be tagged as motivation and will be shown to user
	 */
	public class Motivation
	{
		private static List<Motivation> _lstItems = new List<Motivation>();

		private int        _id;
		private string     _name;
		private BoardItem? _content;

		public Motivation(int id, string nom, BoardItem? item)
		{
			this._id = id;
			this._name = nom;
			this._content = item;
		}


		public static void chargerMotivation(){}

		public static Motivation randomMotivation() 
		{
			return new Motivation(1,"", null);
		}
	}
}
