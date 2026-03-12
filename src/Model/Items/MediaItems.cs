using Avalonia;

namespace Model.Items
{
	/**
	 * AudioItem :
	 * Represents an audio file on the board
	 */
	public class AudioItem : BoardItem
	{
		public string Name { get; set; }
		public string Path { get; set; }

		public AudioItem(string name, string path) : base(0, new Point(), 300, 80, 0)
		{
			this.Name = name;
			this.Path = path;
		}

		public override string ToString() => $"\t-[Audio] {Name}\n";
	}

	/**
	 * VideoItem :
	 * Represents a local video file on the board
	 */
	public class VideoItem : BoardItem
	{
		public string Name { get; set; }
		public string Path { get; set; }

		public VideoItem(string name, string path) : base(0, new Point(), 400, 260, 0)
		{
			this.Name = name;
			this.Path = path;
		}

		public override string ToString() => $"\t-[Video] {Name}\n";
	}
}
