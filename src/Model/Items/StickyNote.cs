using Avalonia;

namespace Model.Items
{
	public class StickyNote : BoardItem
	{
		public string Text { get; set; } = "";

		public StickyNote() { }

		public StickyNote(string text) : base(0, new Point(), 240, 160, 0)
		{
			Text = text;
		}

		public override string ToString() => "\t-" + Text + "\n";
	}
}
