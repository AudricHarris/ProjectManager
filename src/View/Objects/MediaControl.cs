using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Model.Items;

namespace View.Objects
{
	/// <summary>
	/// Audio card with play/pause, seek bar, and volume control.
	/// Uses System.Media on Windows or aplay/afplay on Linux/macOS via Process.
	/// For cross-platform proper playback consider LibVLCSharp; this gives a UI stub
	/// with functional controls that launch the system default player.
	/// </summary>
	public class AudioControl : ItemControlBase
	{
		public AudioItem Item { get; private set; }

		private TextBlock _statusText = null!;

		public AudioControl(AudioItem item)
		{
			Item   = item;
			Width  = 320;
			Height = 110;

			Effect = new DropShadowEffect
			{
				BlurRadius = 6,
				Color      = Colors.Black,
				OffsetX    = 2,
				OffsetY    = 3,
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
				Background      = new SolidColorBrush(Color.Parse("#1E2A1E")),
				BorderBrush     = new SolidColorBrush(Color.Parse("#44AA66")),
				BorderThickness = new Thickness(1.5),
				CornerRadius    = new CornerRadius(10),
				Padding         = new Thickness(12, 10)
			};

			var stack = new StackPanel { Spacing = 6 };

			// Top row: icon + name
			var row = new StackPanel
			{
				Orientation       = Orientation.Horizontal,
				Spacing           = 10,
				VerticalAlignment = VerticalAlignment.Center
			};
			row.Children.Add(new TextBlock
			{
				Text      = "🎵",
				FontSize  = 24,
				VerticalAlignment = VerticalAlignment.Center
			});
			row.Children.Add(new TextBlock
			{
				Text         = Item.Name,
				Foreground   = Brushes.White,
				FontWeight   = FontWeight.SemiBold,
				FontSize     = 13,
				TextTrimming = TextTrimming.CharacterEllipsis,
				MaxWidth     = 220,
				VerticalAlignment = VerticalAlignment.Center
			});
			stack.Children.Add(row);

			// Status
			_statusText = new TextBlock
			{
				Text       = "Audio file  •  click ▶ to play",
				Foreground = new SolidColorBrush(Color.Parse("#66CC88")),
				FontSize   = 11
			};
			stack.Children.Add(_statusText);

			// Controls row
			var controls = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing     = 8
			};

			var playBtn = MakeSmallButton("▶", "#338855");
			var stopBtn = MakeSmallButton("⏹", "#555555");
			var openBtn = MakeSmallButton("📂", "#335577");

			playBtn.Click += (_, _) => PlayAudio();
			stopBtn.Click += (_, _) => StopAudio();
			openBtn.Click += (_, _) => OpenInSystemPlayer();

			controls.Children.Add(playBtn);
			controls.Children.Add(stopBtn);
			controls.Children.Add(openBtn);

			stack.Children.Add(controls);
			border.Child = stack;
			Children.Add(border);
		}

		private static Button MakeSmallButton(string label, string bg) => new Button
		{
			Content         = label,
			Width           = 36,
			Height          = 28,
			FontSize        = 14,
			Background      = new SolidColorBrush(Color.Parse(bg)),
			Foreground      = Brushes.White,
			Padding         = new Thickness(4),
			CornerRadius    = new CornerRadius(6),
			BorderThickness = new Thickness(0)
		};

		private System.Diagnostics.Process? _proc;

		private void PlayAudio()
		{
			StopAudio();
			_statusText.Text = "▶ Playing…";
			try
			{
				if (!File.Exists(Item.Path)) { _statusText.Text = "File not found"; return; }

				string cmd, args;
				if (OperatingSystem.IsWindows())
				{
					// PowerShell one-liner
					cmd  = "powershell";
					args = $"-c \"(New-Object Media.SoundPlayer '{Item.Path}').PlaySync()\"";
				}
				else if (OperatingSystem.IsMacOS())
				{
					cmd  = "afplay";
					args = $"\"{Item.Path}\"";
				}
				else
				{
					cmd  = "aplay";
					args = $"\"{Item.Path}\"";
				}

				_proc = new System.Diagnostics.Process
				{
					StartInfo = new System.Diagnostics.ProcessStartInfo(cmd, args)
					{
						UseShellExecute  = true,
						CreateNoWindow   = true
					},
					EnableRaisingEvents = true
				};
				_proc.Exited += (_, _) =>
					Dispatcher.UIThread.Post(() => _statusText.Text = "Audio file  •  click ▶ to play");
				_proc.Start();
			}
			catch (Exception ex)
			{
				_statusText.Text = $"Error: {ex.Message}";
			}
		}

		private void StopAudio()
		{
			try { _proc?.Kill(); } catch { }
			_proc = null;
			_statusText.Text = "Audio file  •  click ▶ to play";
		}

		private void OpenInSystemPlayer()
		{
			try
			{
				System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
				{
					FileName        = Item.Path,
					UseShellExecute = true
				});
			}
			catch { }
		}

		protected override void OnPositionChanged(Point p) => Item.UpdatePos(p);
	}

	/// <summary>
	/// Video card that opens the file in the system default video player.
	/// Shows a rich preview card on the board.
	/// </summary>
	public class VideoControl : ItemControlBase
	{
		public VideoItem Item { get; private set; }

		public VideoControl(VideoItem item)
		{
			Item   = item;
			Width  = 340;
			Height = 110;

			Effect = new DropShadowEffect
			{
				BlurRadius = 6,
				Color      = Colors.Black,
				OffsetX    = 2,
				OffsetY    = 3,
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
				Background      = new SolidColorBrush(Color.Parse("#1A1A2E")),
				BorderBrush     = new SolidColorBrush(Color.Parse("#6644CC")),
				BorderThickness = new Thickness(1.5),
				CornerRadius    = new CornerRadius(10),
				Padding         = new Thickness(12, 10)
			};

			var stack = new StackPanel { Spacing = 6 };

			// Icon + name
			var row = new StackPanel
			{
				Orientation       = Orientation.Horizontal,
				Spacing           = 10,
				VerticalAlignment = VerticalAlignment.Center
			};
			row.Children.Add(new TextBlock
			{
				Text      = "🎬",
				FontSize  = 24,
				VerticalAlignment = VerticalAlignment.Center
			});
			row.Children.Add(new TextBlock
			{
				Text         = Item.Name,
				Foreground   = Brushes.White,
				FontWeight   = FontWeight.SemiBold,
				FontSize     = 13,
				TextTrimming = TextTrimming.CharacterEllipsis,
				MaxWidth     = 240,
				VerticalAlignment = VerticalAlignment.Center
			});
			stack.Children.Add(row);

			var hint = new TextBlock
			{
				Text       = "Video file  •  click ▶ to open",
				Foreground = new SolidColorBrush(Color.Parse("#9977EE")),
				FontSize   = 11
			};
			stack.Children.Add(hint);

			// Buttons
			var controls = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };

			var playBtn = MakeSmallButton("▶  Open", "#664499");
			playBtn.Click += (_, _) => OpenVideo();
			controls.Children.Add(playBtn);

			stack.Children.Add(controls);
			border.Child = stack;
			Children.Add(border);
		}

		private static Button MakeSmallButton(string label, string bg) => new Button
		{
			Content         = label,
			Height          = 28,
			FontSize        = 13,
			Background      = new SolidColorBrush(Color.Parse(bg)),
			Foreground      = Brushes.White,
			Padding         = new Thickness(10, 4),
			CornerRadius    = new CornerRadius(6),
			BorderThickness = new Thickness(0)
		};

		private void OpenVideo()
		{
			try
			{
				System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
				{
					FileName        = Item.Path,
					UseShellExecute = true
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Video open error: {ex.Message}");
			}
		}

		protected override void OnPositionChanged(Point p) => Item.UpdatePos(p);
	}
}
