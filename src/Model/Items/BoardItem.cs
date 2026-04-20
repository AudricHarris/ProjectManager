using System.Text.Json.Serialization;
using Avalonia;

namespace Model.Items
{
	[JsonDerivedType(typeof(StickyNote),   typeDiscriminator: "StickyNote")]
	[JsonDerivedType(typeof(ImageObject),  typeDiscriminator: "ImageFile")]
	[JsonDerivedType(typeof(UrlEmbed),     typeDiscriminator: "UrlEmbed")]
	[JsonDerivedType(typeof(AudioItem),    typeDiscriminator: "AudioItem")]
	[JsonDerivedType(typeof(VideoItem),    typeDiscriminator: "VideoItem")]
	[JsonDerivedType(typeof(DrawItem),     typeDiscriminator: "DrawItem")]
	public abstract class BoardItem
	{
		public int Id { get; set; }

		// Serialized as flat doubles so System.Text.Json can round-trip them
		public double PosX   { get; set; }
		public double PosY   { get; set; }
		public int    Width  { get; set; }
		public int    Height { get; set; }
		public int    ZIndex { get; set; }

		[JsonIgnore]
		public Point Position => new Point(PosX, PosY);

		protected BoardItem(int id, Point pos, int sizeX, int sizeY, int zIndex)
		{
			Id     = id;
			PosX   = pos.X;
			PosY   = pos.Y;
			Width  = sizeX;
			Height = sizeY;
			ZIndex = zIndex;
		}

		// Parameterless ctor required by System.Text.Json
		protected BoardItem() { }

		public void SetWidth(int w)    { Width  = w; }
		public void SetHeight(int h)   { Height = h; }
		public void UpdatePos(Point p) { PosX = p.X; PosY = p.Y; }

		public bool ContainPoint(Point p) =>
			PosX < p.X && PosY < p.Y &&
			p.X < PosX + Width && p.Y < PosY + Height;
	}
}
