
namespace Model.Items
{

	public abstract class BoardItem
	{
		private int id;
		private Point pos;
		private int sizeX;
		private int sizeY;
		private int zIndex;

		// Constructeurs
		public BoardItem(int id, Point pos, int sizeX, int sizeY, int zIndex)
		{
			this.id = id;
			this.pos = pos;
			this.sizeX = sizeX;
			this.sizeY = sizeY;
			this.zIndex = zIndex;
		}

		// Methode Instances
		public void updatePos(Point p)
		{
			this.pos = p;
		}

		public bool containPoint(Point p)
		{
			return this.pos.X < p.X  && this.pos.Y < p.Y &&
				p.X < this.pos.X + this.sizeX && p.Y < this.pos.Y + this.sizeY; 
		}
	}
}
