using Avalonia;

namespace Model.Items
{
	public class UrlEmbed : BoardItem
	{
		public string Url   { get; set; } = "";
		public string Title { get; set; } = "";

		public UrlEmbed() { }

		public UrlEmbed(string url, string title = "") : base(0, new Point(), 320, 170, 0)
		{
			Url   = url;
			Title = string.IsNullOrEmpty(title) ? url : title;
		}

		public bool IsYouTube =>
			Url.Contains("youtube.com") || Url.Contains("youtu.be");

		public string? YouTubeEmbedId
		{
			get
			{
				if (!IsYouTube) return null;
				if (Url.Contains("youtu.be/"))
					return Url.Split("youtu.be/").LastOrDefault()?.Split('?')[0];
				return Url.Split("v=").LastOrDefault()?.Split('&')[0];
			}
		}

		public override string ToString() => $"\t-[URL] {Title}\n";
	}
}
