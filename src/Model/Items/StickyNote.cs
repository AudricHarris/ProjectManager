namespace Model.Items
{
	public class StickyNote : BoardItem
	{
		private string text;

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
