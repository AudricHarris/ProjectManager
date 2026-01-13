using Model.Items;
namespace Model.Geometry
{
	public class Connection
	{
		private int _id;
		private BoardItem _dep;
		private BoardItem _fin;
		private string    _style;
		
		// Constructor
		public Connection(int id, BoardItem dep, BoardItem fin, string style)
		{
			this._id = id;
			this._dep = dep;
			this._fin = fin;
			this._style = style;
		}
	}
}
