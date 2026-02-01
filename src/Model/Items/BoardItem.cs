using System.Text.Json.Serialization;
using Avalonia;

namespace Model.Items
{
	/**
	 * Board Item:
	 * Abstract class that handles positioning and properties of an item
	 */
	[JsonDerivedType(typeof(StickyNote), typeDiscriminator: "StickyNote")]
	[JsonDerivedType(typeof(ImageObject), typeDiscriminator: "ImageFile")]
	public abstract class BoardItem
	{
		public int Id { get; set; }
		private Point _pos;
		private int _sizeX;
		private int _sizeY;
		private int _zIndex;

		public Point Position => _pos;
		public int Width => _sizeX;
		public int Height => _sizeY;
		public int ZIndex
		{
			get => _zIndex;
			set => _zIndex = value;
		}

		// Constructeurs
		public BoardItem(int id, Point pos, int sizeX, int sizeY, int zIndex)
		{
			this.Id = id;
			this._pos = pos;
			this._sizeX = sizeX;
			this._sizeY = sizeY;
			this._zIndex = zIndex;
		}

		public void SetWidth(int width) { this._sizeX = width; }
		public void SetHeight(int height) { this._sizeY = height; }

		public void UpdatePos(Point p)
		{
			this._pos = p;
		}

		public bool ContainPoint(Point p)
		{
			return this._pos.X < p.X  && this._pos.Y < p.Y &&
				p.X < this._pos.X + this._sizeX && p.Y < this._pos.Y + this._sizeY; 
		}
	}
}
