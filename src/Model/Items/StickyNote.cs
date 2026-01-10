using Model.Geometry;

namespace Model.Items
{
	/**
	 * Sticky note :
	 * Extends board item
	 * handles text objects like list
	 */
	public class StickyNote : BoardItem
	{
		public string text { get; set; }

		public StickyNote(string text) : base(0, new Point(), 0, 0, 0)
		{
			this.text = text;
		}

		override
		public string ToString()
		{
			return "\t-" + this.text + "\n";
		}
	}
}
