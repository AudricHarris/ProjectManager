// Avalonia
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.Platform.Storage;

// Model
using Model.Containers;
using Model.Items;

// View
using View.Objects;
using View.Systems;

namespace View.Panels
{
	public class MindBoardPanel : DockPanel
	{
		private readonly MouseManager _mouseManager;
		private readonly Project      _project;

		private Canvas      _canvas  = null!;
		private StackPanel  _toolbar = null!;

		// Last right-click position on the canvas (for placing new items)
		private Point _contextMenuPos;

		public MindBoardPanel(Project project, MouseManager mouseManager)
		{
			_project      = project      ?? throw new ArgumentNullException(nameof(project));
			_mouseManager = mouseManager ?? throw new ArgumentNullException(nameof(mouseManager));

			Background = SystemStyle.Background;
			MainWindow.SCtrl!.SetProject(project);

			var title = new TextBlock
			{
				Text      = $"Mind Board: {_project.GetName()}",
				FontSize  = 24,
				FontWeight = FontWeight.Bold,
				Margin    = new Thickness(16, 12, 16, 8),
				HorizontalAlignment = HorizontalAlignment.Center,
				Foreground = Brushes.White
			};
			DockPanel.SetDock(title, Dock.Top);
			Children.Add(title);

			var mainGrid = new Grid
			{
				ColumnDefinitions = new ColumnDefinitions("220, *")
			};

			_toolbar = CreateLeftToolbar();
			Grid.SetColumn(_toolbar, 0);
			mainGrid.Children.Add(_toolbar);

			// Scrollable canvas
			var scroll = new ScrollViewer
			{
				HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
				VerticalScrollBarVisibility   = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto
			};

			_canvas = new Canvas
			{
				Width      = 3000,
				Height     = 3000,
				Background = new SolidColorBrush(Color.Parse("#252535")),
				ClipToBounds = false
			};

			// Draw subtle grid lines on the canvas
			_canvas.Children.Insert(0, CreateGridOverlay());

			// Right-click context menu
			_canvas.PointerReleased += Canvas_PointerReleased;

			// Paste from clipboard (Ctrl+V)
			KeyDown += OnKeyDown;

			scroll.Content = _canvas;
			Grid.SetColumn(scroll, 1);
			mainGrid.Children.Add(scroll);

			Children.Add(mainGrid);

			_mouseManager.ItemAdded += OnItemAddedFromManager;
			LoadExistingItems();
		}

		private Control CreateGridOverlay()
		{
			// We draw it as a very subtle tiled background via a custom control
			return new GridBackground { Width = 3000, Height = 3000 };
		}

		private StackPanel CreateLeftToolbar()
		{
			var panel = new StackPanel
			{
				Margin      = new Thickness(0),
				Spacing     = 0,
				Orientation = Orientation.Vertical,
				Background  = SystemStyle.TopBanner
			};

			// Project name header
			var header = new Border
			{
				Padding    = new Thickness(16, 20, 16, 16),
				Background = new SolidColorBrush(Color.Parse("#1E1E2E"))
			};
			var headerText = new TextBlock
			{
				Text       = _project.GetName() ?? "Untitled",
				FontSize   = 16,
				FontWeight = FontWeight.SemiBold,
				Foreground = Brushes.White,
				TextWrapping = TextWrapping.Wrap
			};
			header.Child = headerText;
			panel.Children.Add(header);

			// Section label
			panel.Children.Add(MakeLabel("ADD ITEMS"));

			// Buttons
			panel.Children.Add(MakeToolButton("📝  Sticky Note",    "#E6B800", AddStickyNote));
			panel.Children.Add(MakeToolButton("🖼️  Image File",     "#557799", AddImageFile));
			panel.Children.Add(MakeToolButton("🔗  URL / Embed",    "#5555AA", AddUrlEmbed));
			panel.Children.Add(MakeToolButton("🎵  Audio File",     "#338855", AddAudioFile));
			panel.Children.Add(MakeToolButton("🎬  Video File",     "#664499", AddVideoFile));

			panel.Children.Add(MakeLabel("CANVAS"));
			panel.Children.Add(MakeToolButton("🗑️  Clear All",      "#AA3333", ClearAll));

			// Tip box at the bottom
			var tip = new Border
			{
				Margin     = new Thickness(12, 16, 12, 0),
				Padding    = new Thickness(10),
				Background = new SolidColorBrush(Color.Parse("#1A1A2E")),
				CornerRadius = new CornerRadius(8),
				Child = new TextBlock
				{
					Text         = "💡 Right-click on the canvas for a context menu.\n\nCtrl+V to paste images from clipboard.\n\nDouble-click notes to edit.",
					FontSize     = 11,
					Foreground   = new SolidColorBrush(Color.Parse("#8888AA")),
					TextWrapping = TextWrapping.Wrap
				}
			};
			panel.Children.Add(tip);

			return panel;
		}

		private static TextBlock MakeLabel(string text) => new TextBlock
		{
			Text      = text,
			Foreground = new SolidColorBrush(Color.Parse("#666688")),
			FontSize   = 10,
			FontWeight = FontWeight.SemiBold,
			Margin    = new Thickness(16, 16, 16, 4)
		};

		private static Button MakeToolButton(string label, string accent, Action handler)
		{
			var btn = new Button
			{
				Content             = label,
				FontSize            = 13,
				Foreground          = Brushes.White,
				Background          = Brushes.Transparent,
				BorderThickness     = new Thickness(0, 0, 3, 0),
				BorderBrush         = new SolidColorBrush(Color.Parse(accent)),
				Padding             = new Thickness(16, 10),
				Margin              = new Thickness(0, 1),
				HorizontalContentAlignment = HorizontalAlignment.Left,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				CornerRadius        = new CornerRadius(0)
			};
			btn.Click += (_, _) => handler();
			return btn;
		}

		private void Canvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
		{
			if (e.InitialPressMouseButton == MouseButton.Right)
			{
				_contextMenuPos = e.GetPosition(_canvas);
				ShowContextMenu(e.GetPosition(this));
				e.Handled = true;
			}
		}

		private void ShowContextMenu(Point screenPos)
		{
			var menu = new ContextMenu();

			MenuItem Make(string header, Action action)
			{
				var mi = new MenuItem { Header = header };
				mi.Click += (_, _) => action();
				return mi;
			}

			menu.Items.Add(Make("📝  Add Sticky Note",  AddStickyNoteAt));
			menu.Items.Add(Make("🖼️  Add Image...",      AddImageFile));
			menu.Items.Add(Make("🔗  Add URL / Embed…", AddUrlEmbed));
			menu.Items.Add(Make("🎵  Add Audio File…",  AddAudioFile));
			menu.Items.Add(Make("🎬  Add Video File…",  AddVideoFile));
			menu.Items.Add(new Separator());
			menu.Items.Add(Make("🗑️  Clear All Items",  ClearAll));

			menu.Open(_canvas);
		}

		private async void OnKeyDown(object? sender, KeyEventArgs e)
		{
			if (e.Key == Key.V && e.KeyModifiers.HasFlag(KeyModifiers.Control))
			{
				await TryPasteImageAsync();
			}
		}

		private async Task TryPasteImageAsync()
		{
			try
			{
				var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
				if (clipboard == null) return;

				var formats = await clipboard.GetFormatsAsync();
				if (formats == null) return;

				// Check for image data
				bool hasImage = formats.Any(f =>
					f.Contains("image", StringComparison.OrdinalIgnoreCase) ||
					f.Contains("png",   StringComparison.OrdinalIgnoreCase) ||
					f.Contains("bitmap",StringComparison.OrdinalIgnoreCase));

				if (!hasImage) return;

				// Try to get the image bytes
				byte[]? data = null;
				foreach (var fmt in formats.Where(f =>
					f.Contains("image", StringComparison.OrdinalIgnoreCase) ||
					f.Contains("png",   StringComparison.OrdinalIgnoreCase)))
				{
					var raw = await clipboard.GetDataAsync(fmt);
					if (raw is byte[] bytes && bytes.Length > 0)
					{
						data = bytes;
						break;
					}
				}

				if (data == null) return;

				// Save to a temp file
				string tmpDir  = Path.Combine(Path.GetTempPath(), "MindBoard");
				Directory.CreateDirectory(tmpDir);
				string tmpFile = Path.Combine(tmpDir, $"paste_{DateTime.Now:yyyyMMdd_HHmmss}.png");
				await File.WriteAllBytesAsync(tmpFile, data);

				var imgItem = new ImageObject("Pasted Image", tmpFile);
				double x = _canvas.Children.Count * 20 % 600 + 60;
				double y = _canvas.Children.Count * 20 % 400 + 60;
				imgItem.UpdatePos(new Point(x, y));
				imgItem.SetWidth(240);
				imgItem.SetHeight(180);

				_mouseManager.NotifyNewItemCreated(imgItem);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Paste error: {ex.Message}");
			}
		}

		private void AddStickyNote()
		{
			double offset = 40 + _canvas.Children.Count * 24 % 500;
			AddStickyNoteAtPos(new Point(offset, offset));
		}

		private void AddStickyNoteAt()
		{
			AddStickyNoteAtPos(_contextMenuPos);
		}

		private void AddStickyNoteAtPos(Point pos)
		{
			var note = new StickyNote("New note…\n\nDouble-click to edit.");
			note.UpdatePos(pos);
			_mouseManager.NotifyNewItemCreated(note);
		}

		private async void AddImageFile()
		{
			try
			{
				var topLevel = TopLevel.GetTopLevel(this);
				if (topLevel == null) return;

				var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
				{
					Title          = "Select Image",
					AllowMultiple  = false,
					FileTypeFilter = new[]
					{
						new FilePickerFileType("Images")
						{
							Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.gif", "*.bmp", "*.webp" }
						}
					}
				});

				if (files.Count == 0) return;

				var file = files[0];
				string path = file.Path.LocalPath;
				string name = Path.GetFileName(path);

				var imgItem = new ImageObject(name, path);
				imgItem.UpdatePos(_contextMenuPos == default ? new Point(60, 60) : _contextMenuPos);
				imgItem.SetWidth(240);
				imgItem.SetHeight(200);

				_mouseManager.NotifyNewItemCreated(imgItem);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Image picker error: {ex.Message}");
			}
		}

		private async void AddUrlEmbed()
		{
			var popup = new Window
			{
				Title                 = "Add URL / Embed",
				Width                 = 440,
				Height                = 220,
				Background            = new SolidColorBrush(Color.Parse("#2D2D3D")),
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			};

			var urlBox = new TextBox
			{
				Watermark  = "https://youtube.com/watch?v=... or any URL",
				FontSize   = 14,
				Foreground = Brushes.White,
				Background = new SolidColorBrush(Color.Parse("#3D3D50")),
				Margin     = new Thickness(16, 12, 16, 8)
			};

			var titleBox = new TextBox
			{
				Watermark  = "Display title (optional)",
				FontSize   = 14,
				Foreground = Brushes.White,
				Background = new SolidColorBrush(Color.Parse("#3D3D50")),
				Margin     = new Thickness(16, 0, 16, 12)
			};

			var add = new Button
			{
				Content             = "Add Embed",
				FontSize            = 14,
				FontWeight          = FontWeight.SemiBold,
				Foreground          = Brushes.White,
				Background          = new SolidColorBrush(Color.Parse("#5555CC")),
				Margin              = new Thickness(16, 0, 16, 16),
				Padding             = new Thickness(12, 8),
				CornerRadius        = new CornerRadius(8),
				HorizontalAlignment = HorizontalAlignment.Stretch
			};

			add.Click += (_, _) =>
			{
				string url   = urlBox.Text?.Trim() ?? "";
				string title = titleBox.Text?.Trim() ?? "";
				if (!string.IsNullOrEmpty(url))
				{
					var embed = new UrlEmbed(url, string.IsNullOrEmpty(title) ? url : title);
					embed.UpdatePos(_contextMenuPos == default ? new Point(80, 80) : _contextMenuPos);
					_mouseManager.NotifyNewItemCreated(embed);
				}
				popup.Close();
			};

			var panel = new StackPanel();
			panel.Children.Add(new TextBlock
			{
				Text = "Paste a URL — YouTube links will show as video embeds:",
				Foreground = Brushes.LightGray,
				FontSize = 12,
				Margin = new Thickness(16, 12, 16, 4)
			});
			panel.Children.Add(urlBox);
			panel.Children.Add(titleBox);
			panel.Children.Add(add);
			popup.Content = panel;
			await popup.ShowDialog(TopLevel.GetTopLevel(this) as Window ?? throw new Exception());
		}

		private async void AddAudioFile()
		{
			try
			{
				var topLevel = TopLevel.GetTopLevel(this);
				if (topLevel == null) return;

				var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
				{
					Title          = "Select Audio File",
					AllowMultiple  = false,
					FileTypeFilter = new[]
					{
						new FilePickerFileType("Audio")
						{
							Patterns = new[] { "*.mp3", "*.wav", "*.ogg", "*.flac", "*.aac", "*.m4a" }
						}
					}
				});

				if (files.Count == 0) return;
				var file = files[0];

				var audio = new AudioItem(Path.GetFileName(file.Path.LocalPath), file.Path.LocalPath);
				audio.UpdatePos(_contextMenuPos == default ? new Point(80, 80) : _contextMenuPos);
				_mouseManager.NotifyNewItemCreated(audio);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Audio picker error: {ex.Message}");
			}
		}

		private async void AddVideoFile()
		{
			try
			{
				var topLevel = TopLevel.GetTopLevel(this);
				if (topLevel == null) return;

				var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
				{
					Title          = "Select Video File",
					AllowMultiple  = false,
					FileTypeFilter = new[]
					{
						new FilePickerFileType("Video")
						{
							Patterns = new[] { "*.mp4", "*.mkv", "*.avi", "*.mov", "*.webm", "*.wmv" }
						}
					}
				});

				if (files.Count == 0) return;
				var file = files[0];

				var vid = new VideoItem(Path.GetFileName(file.Path.LocalPath), file.Path.LocalPath);
				vid.UpdatePos(_contextMenuPos == default ? new Point(80, 80) : _contextMenuPos);
				_mouseManager.NotifyNewItemCreated(vid);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Video picker error: {ex.Message}");
			}
		}

		private void ClearAll()
		{
			// Remove non-grid-background children
			var toRemove = _canvas.Children
				.OfType<Control>()
				.Where(c => c is not GridBackground)
				.ToList();

			foreach (var c in toRemove)
				_canvas.Children.Remove(c);

			_project.GetLstItemProject().Clear();
			MainWindow.SCtrl!.SaveProject();
		}

		private void LoadExistingItems()
		{
			foreach (var item in _project.GetLstItemProject())
				RenderItem(item);
		}

		private void OnItemAddedFromManager(BoardItem item)
		{
			_project.AddItem(item);
			RenderItem(item);
			Console.WriteLine("Item added → saving project");
			MainWindow.SCtrl!.SaveProject();
		}

		private void RenderItem(BoardItem item)
		{
			if (item == null) return;

			Control? visual = item switch
			{
				StickyNote   note  => new StickyNoteControl { Item = note },
				ImageObject  img   => new ImageControl(img),
				UrlEmbed     url   => new UrlEmbedControl(url),
				AudioItem    audio => new AudioControl(audio),
				VideoItem    vid   => new VideoControl(vid),
				_ => new Border
				{
					Child = new TextBlock
					{
						Text       = $"[Unsupported: {item.GetType().Name}]",
						Foreground = Brushes.Red
					},
					Background = Brushes.LightPink,
					Padding    = new Thickness(8)
				}
			};

			if (visual != null)
			{
				Canvas.SetLeft(visual, item.Position.X);
				Canvas.SetTop(visual, item.Position.Y);
				_canvas.Children.Add(visual);
			}
		}

		protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
		{
			base.OnDetachedFromVisualTree(e);
			_mouseManager.ItemAdded -= OnItemAddedFromManager;
		}
	}

	internal class GridBackground : Control
	{
		public override void Render(DrawingContext ctx)
		{
			base.Render(ctx);
			var dotBrush = new SolidColorBrush(Color.FromArgb(40, 120, 120, 180));
			const double step = 40;

			for (double x = 0; x < Bounds.Width; x += step)
			for (double y = 0; y < Bounds.Height; y += step)
				ctx.DrawEllipse(dotBrush, null, new Point(x, y), 1.5, 1.5);
		}
	}
}
