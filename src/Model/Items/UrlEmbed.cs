using System.Text.Json.Serialization;
using Avalonia;

namespace Model.Items
{
	/**
	 * UrlEmbed :
	 * Embeds a URL (YouTube, website, etc.) as a card on the board
	 */
	public class UrlEmbed : BoardItem
	{
		public string Url   { get; set; }
		public string Title { get; set; }

		public UrlEmbed(string url, string title = "") : base(0, new Point(), 320, 220, 0)
		{
			this.Url   = url;
			this.Title = string.IsNullOrEmpty(title) ? url : title;
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
				var v = Url.Split("v=").LastOrDefault()?.Split('&')[0];
				return v;
			}
		}

		public override string ToString() => $"\t-[URL] {Title}\n";
	}
}
