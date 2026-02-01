using Model.Geometry;
using Avalonia;

namespace Model.Items
{
	/**
	 * Sticky note :
	 * Extends board item
	 * handles text objects like list
	 */
	public class StickyNote : BoardItem
	{
		public string Text { get; set; }

		public StickyNote(string text) : base(0, new Point(), 0, 0, 0)
		{
			this.Text = text;
		}

		override
		public string ToString()
		{
			return "\t-" + this.Text + "\n";
		}
	}
}
