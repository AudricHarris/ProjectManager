using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Layout;
using Model.Items;

namespace View.Objects
{
	/// <summary>
	/// URL embed card. Clicking the link button opens the URL in the default browser.
	/// YouTube links show the embed ID and a dedicated "Open on YouTube" button.
	/// </summary>
	public class UrlEmbedControl : ItemControlBase
	{
		public UrlEmbed Item { get; private set; }

		public UrlEmbedControl(UrlEmbed item)
		{
			Item   = item;
			Width  = 320;
			Height = item.IsYouTube ? 170 : 120;

			Effect = new DropShadowEffect
			{
				BlurRadius = 8,
				Color      = Colors.Black,
				OffsetX    = 2,
				OffsetY    = 4,
				Opacity    = 0.3
			};

			Canvas.SetLeft(this, item.Position.X);
			Canvas.SetTop(this, item.Position.Y);

			Build();
			Cursor = new Cursor(StandardCursorType.SizeAll);
			InitHandles();
		}

		private void Build()
		{
			var border = new Border
			{
				Background      = new SolidColorBrush(Color.Parse("#1E1E2E")),
				BorderBrush     = new SolidColorBrush(Color.Parse("#5555AA")),
				BorderThickness = new Thickness(1.5),
				CornerRadius    = new CornerRadius(10),
				Padding         = new Thickness(14)
			};

			var stack = new StackPanel { Spacing = 8 };

			// Icon + type
			var header = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing     = 8
			};
			string icon = Item.IsYouTube ? "▶️" : "🔗";
			header.Children.Add(new TextBlock
			{
				Text      = icon,
				FontSize  = 20,
				VerticalAlignment = VerticalAlignment.Center
			});
			header.Children.Add(new TextBlock
			{
				Text       = Item.IsYouTube ? "YouTube Video" : "Link",
				Foreground = new SolidColorBrush(Color.Parse("#AAAAFF")),
				FontSize   = 13,
				FontWeight = FontWeight.SemiBold,
				VerticalAlignment = VerticalAlignment.Center
			});
			stack.Children.Add(header);

			// Title
			stack.Children.Add(new TextBlock
			{
				Text         = Item.Title,
				Foreground   = Brushes.White,
				FontSize     = 14,
				FontWeight   = FontWeight.Bold,
				TextWrapping = TextWrapping.Wrap,
				MaxWidth     = 280
			});

			// URL (truncated display)
			var urlDisplay = Item.Url.Length > 48 ? Item.Url.Substring(0, 45) + "…" : Item.Url;
			stack.Children.Add(new TextBlock
			{
				Text       = urlDisplay,
				Foreground = new SolidColorBrush(Color.Parse("#777799")),
				FontSize   = 11
			});

			// Buttons row
			var buttons = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };

			if (Item.IsYouTube)
			{
				var ytBtn = MakeActionButton("▶  Open on YouTube", "#CC2222");
				ytBtn.Click += (_, _) => OpenUrl(Item.Url);
				buttons.Children.Add(ytBtn);

				if (Item.YouTubeEmbedId != null)
				{
					stack.Children.Add(new TextBlock
					{
						Text       = $"🎬  ID: {Item.YouTubeEmbedId}",
						Foreground = new SolidColorBrush(Color.Parse("#FF6666")),
						FontSize   = 11
					});
				}
			}
			else
			{
				var linkBtn = MakeActionButton("🔗  Open Link", "#444499");
				linkBtn.Click += (_, _) => OpenUrl(Item.Url);
				buttons.Children.Add(linkBtn);
			}

			stack.Children.Add(buttons);
			border.Child = stack;
			Children.Add(border);
		}

		private static Button MakeActionButton(string label, string bg) => new Button
		{
			Content         = label,
			Height          = 28,
			FontSize        = 12,
			FontWeight      = FontWeight.SemiBold,
			Background      = new SolidColorBrush(Color.Parse(bg)),
			Foreground      = Brushes.White,
			Padding         = new Thickness(10, 4),
			CornerRadius    = new CornerRadius(6),
			BorderThickness = new Thickness(0)
		};

		private static void OpenUrl(string url)
		{
			try
			{
				System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
				{
					FileName        = url,
					UseShellExecute = true
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Open URL error: {ex.Message}");
			}
		}

		protected override void OnPositionChanged(Point p) => Item.UpdatePos(p);
	}
}
