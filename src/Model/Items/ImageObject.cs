using Avalonia;

namespace Model.Items
{
	public class ImageObject : BoardItem
	{
		public string Name { get; set; } = "";
		public string Path { get; set; } = "";

		public ImageObject() { }

		public ImageObject(string name, string path) : base(0, new Point(), 240, 180, 0)
		{
			Name = name;
			Path = path;
		}

		public override string ToString() => "\t-" + Name + "\n";
	}
}
