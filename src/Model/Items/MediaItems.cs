using Avalonia;

namespace Model.Items
{
	public class AudioItem : BoardItem
	{
		public string Name { get; set; } = "";
		public string Path { get; set; } = "";

		public AudioItem() { }

		public AudioItem(string name, string path) : base(0, new Point(), 300, 110, 0)
		{
			Name = name;
			Path = path;
		}

		public override string ToString() => $"\t-[Audio] {Name}\n";
	}

	public class VideoItem : BoardItem
	{
		public string Name { get; set; } = "";
		public string Path { get; set; } = "";

		public VideoItem() { }

		public VideoItem(string name, string path) : base(0, new Point(), 340, 110, 0)
		{
			Name = name;
			Path = path;
		}

		public override string ToString() => $"\t-[Video] {Name}\n";
	}
}
