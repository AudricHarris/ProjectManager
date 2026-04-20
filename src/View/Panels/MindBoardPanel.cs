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
	// ── Tool mode ────────────────────────────────────────────────────────
	public enum ToolMode { Select, DrawFreehand, DrawLine, DrawRect, DrawEllipse }

	public class MindBoardPanel : DockPanel
	{
		private readonly MouseManager _mouseManager;
		private readonly Project      _project;

		private Canvas     _canvas  = null!;
		private StackPanel _toolbar = null!;

		private Point _contextMenuPos;

		// ── Draw state ───────────────────────────────────────────────────
		private ToolMode _toolMode      = ToolMode.Select;
		private bool     _isDrawing     = false;
		private Point    _drawStart;
		private List<Point> _freePoints = new();

		// Live preview overlay while drawing a shape
		private DrawPreviewControl? _preview;

		// Draw settings
		private string _drawColor       = "#FFFFFF";
		private double _drawStrokeWidth = 3;

		// Toolbar buttons (kept for toggle highlight)
		private readonly Dictionary<ToolMode, Button> _toolButtons = new();

		public MindBoardPanel(Project project, MouseManager mouseManager)
		{
			_project      = project      ?? throw new ArgumentNullException(nameof(project));
			_mouseManager = mouseManager ?? throw new ArgumentNullException(nameof(mouseManager));

			Background = SystemStyle.Background;
			MainWindow.SCtrl!.SetProject(project);

			var title = new TextBlock
			{
				Text       = $"Mind Board: {_project.GetName()}",
				FontSize   = 24,
				FontWeight = FontWeight.Bold,
				Margin     = new Thickness(16, 12, 16, 8),
				HorizontalAlignment = HorizontalAlignment.Center,
				Foreground = Brushes.White
			};
			DockPanel.SetDock(title, Dock.Top);
			Children.Add(title);

			var mainGrid = new Grid
			{
				ColumnDefinitions = new ColumnDefinitions("240, *")
			};

			_toolbar = CreateLeftToolbar();
			Grid.SetColumn(_toolbar, 0);
			mainGrid.Children.Add(_toolbar);

			var scroll = new ScrollViewer
			{
				HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
				VerticalScrollBarVisibility   = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto
			};

			_canvas = new Canvas
			{
				Width        = 3000,
				Height       = 3000,
				Background   = new SolidColorBrush(Color.Parse("#252535")),
				ClipToBounds = false
			};

			_canvas.Children.Insert(0, CreateGridOverlay());
			_canvas.PointerPressed  += Canvas_PointerPressed;
			_canvas.PointerMoved    += Canvas_PointerMoved;
			_canvas.PointerReleased += Canvas_PointerReleased;

			KeyDown += OnKeyDown;

			scroll.Content = _canvas;
			Grid.SetColumn(scroll, 1);
			mainGrid.Children.Add(scroll);

			Children.Add(mainGrid);

			_mouseManager.ItemAdded += OnItemAddedFromManager;
			LoadExistingItems();
		}

		private Control CreateGridOverlay() =>
			new GridBackground { Width = 3000, Height = 3000 };

		// ─────────────────────────────────────────────────────────────────
		//  TOOLBAR
		// ─────────────────────────────────────────────────────────────────

		private StackPanel CreateLeftToolbar()
		{
			var panel = new StackPanel
			{
				Margin      = new Thickness(0),
				Spacing     = 0,
				Orientation = Orientation.Vertical,
				Background  = SystemStyle.TopBanner
			};

			// ── Back to menu button ────────────────────────────────────
			var backBtn = new Button
			{
				Content             = "← Back to Menu",
				FontSize            = 13,
				FontWeight          = FontWeight.SemiBold,
				Foreground          = Brushes.White,
				Background          = new SolidColorBrush(Color.Parse("#333344")),
				BorderThickness     = new Thickness(0, 0, 0, 1),
				BorderBrush         = new SolidColorBrush(Color.Parse("#444455")),
				Padding             = new Thickness(16, 14),
				HorizontalContentAlignment = HorizontalAlignment.Left,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				CornerRadius        = new CornerRadius(0)
			};
			backBtn.Click += (_, _) =>
			{
				MainWindow.SCtrl?.SaveProject();
				_mouseManager.CancelProject();
			};
			panel.Children.Add(backBtn);

			// Project name header
			var header = new Border
			{
				Padding    = new Thickness(16, 14, 16, 14),
				Background = new SolidColorBrush(Color.Parse("#1E1E2E"))
			};
			header.Child = new TextBlock
			{
				Text         = _project.GetName() ?? "Untitled",
				FontSize     = 15,
				FontWeight   = FontWeight.SemiBold,
				Foreground   = Brushes.White,
				TextWrapping = TextWrapping.Wrap
			};
			panel.Children.Add(header);

			// ── ADD ITEMS ──────────────────────────────────────────────
			panel.Children.Add(MakeLabel("ADD ITEMS"));
			panel.Children.Add(MakeToolButton("📝  Sticky Note",  "#E6B800", AddStickyNote));
			panel.Children.Add(MakeToolButton("🖼️  Image File",   "#557799", AddImageFile));
			panel.Children.Add(MakeToolButton("🔗  URL / Embed",  "#5555AA", AddUrlEmbed));
			panel.Children.Add(MakeToolButton("🎵  Audio File",   "#338855", AddAudioFile));
			panel.Children.Add(MakeToolButton("🎬  Video File",   "#664499", AddVideoFile));

			// ── DRAW TOOLS ─────────────────────────────────────────────
			panel.Children.Add(MakeLabel("DRAW TOOLS"));
			AddDrawModeButton(panel, "🖊️  Freehand",  "#AA7744", ToolMode.DrawFreehand);
			AddDrawModeButton(panel, "╱  Line",       "#8888AA", ToolMode.DrawLine);
			AddDrawModeButton(panel, "▭  Rectangle",  "#448877", ToolMode.DrawRect);
			AddDrawModeButton(panel, "◯  Ellipse",    "#774488", ToolMode.DrawEllipse);

			// Color picker + stroke
			panel.Children.Add(MakeLabel("DRAW STYLE"));
			panel.Children.Add(BuildColorRow());
			panel.Children.Add(BuildStrokeRow());

			// ── CANVAS ────────────────────────────────────────────────
			panel.Children.Add(MakeLabel("CANVAS"));
			panel.Children.Add(MakeToolButton("🗑️  Clear All", "#AA3333", ClearAll));

			// Tip
			var tip = new Border
			{
				Margin       = new Thickness(12, 16, 12, 0),
				Padding      = new Thickness(10),
				Background   = new SolidColorBrush(Color.Parse("#1A1A2E")),
				CornerRadius = new CornerRadius(8),
				Child = new TextBlock
				{
					Text         = "💡 Right-click canvas for menu.\n\nCtrl+V paste image.\n\nDouble-click notes to edit.\n\n✕ = delete item\n↻ = rotate item",
					FontSize     = 11,
					Foreground   = new SolidColorBrush(Color.Parse("#8888AA")),
					TextWrapping = TextWrapping.Wrap
				}
			};
			panel.Children.Add(tip);

			return panel;
		}

		private void AddDrawModeButton(StackPanel panel, string label, string accent, ToolMode mode)
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
			btn.Click += (_, _) => SetToolMode(mode, btn);
			_toolButtons[mode] = btn;
			panel.Children.Add(btn);
		}

		private Control BuildColorRow()
		{
			var row = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing     = 6,
				Margin      = new Thickness(12, 4, 12, 4)
			};

			string[] colors = { "#FFFFFF", "#FF4444", "#44FF88", "#4488FF",
			                    "#FFCC00", "#FF88FF", "#00CCFF", "#FF8844" };

			foreach (var c in colors)
			{
				var swatch = new Border
				{
					Width        = 20,
					Height       = 20,
					Background   = new SolidColorBrush(Color.Parse(c)),
					CornerRadius = new CornerRadius(10),
					BorderBrush  = Brushes.Transparent,
					BorderThickness = new Thickness(2),
					Cursor       = new Cursor(StandardCursorType.Hand)
				};
				string captured = c;
				swatch.PointerPressed += (_, e) =>
				{
					_drawColor = captured;
					// Highlight selected
					foreach (var child in row.Children.OfType<Border>())
						child.BorderBrush = Brushes.Transparent;
					swatch.BorderBrush = Brushes.White;
					e.Handled = true;
				};
				row.Children.Add(swatch);
			}

			return row;
		}

		private Control BuildStrokeRow()
		{
			var row = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing     = 8,
				Margin      = new Thickness(12, 2, 12, 4),
				VerticalAlignment = VerticalAlignment.Center
			};
			row.Children.Add(new TextBlock
			{
				Text       = "Stroke:",
				Foreground = Brushes.LightGray,
				FontSize   = 11,
				VerticalAlignment = VerticalAlignment.Center
			});

			var slider = new Slider
			{
				Minimum = 1, Maximum = 20, Value = _drawStrokeWidth,
				Width   = 100
			};
			slider.ValueChanged += (_, e) => _drawStrokeWidth = e.NewValue;
			row.Children.Add(slider);
			return row;
		}

		private void SetToolMode(ToolMode mode, Button? btn = null)
		{
			_toolMode = mode;

			// Visual: reset all draw buttons, highlight active
			foreach (var (m, b) in _toolButtons)
			{
				b.Background = m == mode
					? new SolidColorBrush(Color.Parse("#333355"))
					: Brushes.Transparent;
			}

			_canvas.Cursor = mode == ToolMode.Select
				? Cursor.Default
				: new Cursor(StandardCursorType.Cross);
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

		// ─────────────────────────────────────────────────────────────────
		//  CANVAS INPUT — draw + right-click
		// ─────────────────────────────────────────────────────────────────

		private void Canvas_PointerPressed(object? sender, PointerPressedEventArgs e)
		{
			var pt    = e.GetPosition(_canvas);
			var props = e.GetCurrentPoint(_canvas).Properties;

			if (props.IsRightButtonPressed)
			{
				_contextMenuPos = pt;
				ShowContextMenu();
				e.Handled = true;
				return;
			}

			if (_toolMode == ToolMode.Select)
			{
				// Click on the canvas background — deselect everything
				if (e.Source == _canvas || e.Source is GridBackground)
					ItemControlBase.DeselectAll();
				return;
			}

			if (props.IsLeftButtonPressed)
			{
				_isDrawing  = true;
				_drawStart  = pt;
				_freePoints = new List<Point> { pt };

				if (_toolMode != ToolMode.DrawFreehand)
				{
					// Add live preview overlay
					_preview = new DrawPreviewControl(_drawStart, _toolMode, _drawColor, _drawStrokeWidth);
					_canvas.Children.Add(_preview);
				}

				e.Pointer.Capture(_canvas);
				e.Handled = true;
			}
		}

		private void Canvas_PointerMoved(object? sender, PointerEventArgs e)
		{
			if (!_isDrawing) return;
			var pt = e.GetPosition(_canvas);

			if (_toolMode == ToolMode.DrawFreehand)
			{
				_freePoints.Add(pt);
				// Draw incremental line segment on the canvas directly via a temp approach:
				// We'll finalize everything on release; for live feedback add tiny ellipses
				var dot = new Avalonia.Controls.Shapes.Ellipse
				{
					Width             = _drawStrokeWidth,
					Height            = _drawStrokeWidth,
					Fill              = new SolidColorBrush(Color.Parse(_drawColor)),
					IsHitTestVisible  = false,
					Tag               = "draw_dot"
				};
				Canvas.SetLeft(dot, pt.X - _drawStrokeWidth / 2);
				Canvas.SetTop(dot,  pt.Y - _drawStrokeWidth / 2);
				_canvas.Children.Add(dot);
			}
			else if (_preview != null)
			{
				_preview.UpdateEnd(pt);
			}

			e.Handled = true;
		}

		private void Canvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
		{
			if (_toolMode != ToolMode.Select && e.InitialPressMouseButton == MouseButton.Left && _isDrawing)
			{
				_isDrawing = false;
				var endPt = e.GetPosition(_canvas);

				// Remove preview and temp dots
				RemoveDrawDots();
				if (_preview != null)
				{
					_canvas.Children.Remove(_preview);
					_preview = null;
				}

				List<Point> pts;
				DrawShape shape;

				if (_toolMode == ToolMode.DrawFreehand)
				{
					pts   = _freePoints;
					shape = DrawShape.Freehand;
				}
				else
				{
					pts   = new List<Point> { _drawStart, endPt };
					shape = _toolMode switch
					{
						ToolMode.DrawLine    => DrawShape.Line,
						ToolMode.DrawRect    => DrawShape.Rectangle,
						ToolMode.DrawEllipse => DrawShape.Ellipse,
						_                   => DrawShape.Line
					};
				}

				if (pts.Count >= 2)
				{
					var draw = new DrawItem(shape, pts, _drawColor, _drawStrokeWidth);
					_mouseManager.NotifyNewItemCreated(draw);
				}

				e.Pointer.Capture(null);
				e.Handled = true;
				return;
			}

			if (e.InitialPressMouseButton == MouseButton.Right && _toolMode == ToolMode.Select)
			{
				// Already handled in Pressed
			}
		}

		private void RemoveDrawDots()
		{
			var toRemove = _canvas.Children
				.OfType<Avalonia.Controls.Shapes.Ellipse>()
				.Where(el => el.Tag is string s && s == "draw_dot")
				.ToList();
			foreach (var d in toRemove)
				_canvas.Children.Remove(d);
		}

		// ─────────────────────────────────────────────────────────────────
		//  CONTEXT MENU
		// ─────────────────────────────────────────────────────────────────

		private void ShowContextMenu()
		{
			var menu = new ContextMenu();

			MenuItem Make(string header, Action action)
			{
				var mi = new MenuItem { Header = header };
				mi.Click += (_, _) => action();
				return mi;
			}

			menu.Items.Add(Make("📝  Add Sticky Note",  AddStickyNoteAt));
			menu.Items.Add(Make("🖼️  Add Image…",        AddImageFile));
			menu.Items.Add(Make("🔗  Add URL / Embed…", AddUrlEmbed));
			menu.Items.Add(Make("🎵  Add Audio File…",  AddAudioFile));
			menu.Items.Add(Make("🎬  Add Video File…",  AddVideoFile));
			menu.Items.Add(new Separator());
			menu.Items.Add(Make("🗑️  Clear All Items",  ClearAll));

			menu.Open(_canvas);
		}

		// ─────────────────────────────────────────────────────────────────
		//  ITEM ACTIONS
		// ─────────────────────────────────────────────────────────────────

		private async void OnKeyDown(object? sender, KeyEventArgs e)
		{
			if (e.Key == Key.V && e.KeyModifiers.HasFlag(KeyModifiers.Control))
				await TryPasteImageAsync();
		}

		private async Task TryPasteImageAsync()
		{
			try
			{
				var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
				if (clipboard == null) return;
				var formats = await clipboard.GetFormatsAsync();
				if (formats == null) return;

				bool hasImage = formats.Any(f =>
					f.Contains("image", StringComparison.OrdinalIgnoreCase) ||
					f.Contains("png",   StringComparison.OrdinalIgnoreCase) ||
					f.Contains("bitmap",StringComparison.OrdinalIgnoreCase));
				if (!hasImage) return;

				byte[]? data = null;
				foreach (var fmt in formats.Where(f =>
					f.Contains("image", StringComparison.OrdinalIgnoreCase) ||
					f.Contains("png",   StringComparison.OrdinalIgnoreCase)))
				{
					var raw = await clipboard.GetDataAsync(fmt);
					if (raw is byte[] bytes && bytes.Length > 0) { data = bytes; break; }
				}
				if (data == null) return;

				string tmpDir  = Path.Combine(Path.GetTempPath(), "MindBoard");
				Directory.CreateDirectory(tmpDir);
				string tmpFile = Path.Combine(tmpDir, $"paste_{DateTime.Now:yyyyMMdd_HHmmss}.png");
				await File.WriteAllBytesAsync(tmpFile, data);

				var imgItem = new ImageObject("Pasted Image", tmpFile);
				imgItem.UpdatePos(new Point(60, 60));
				imgItem.SetWidth(240);
				imgItem.SetHeight(180);
				_mouseManager.NotifyNewItemCreated(imgItem);
			}
			catch (Exception ex) { Console.WriteLine($"Paste error: {ex.Message}"); }
		}

		private void AddStickyNote()    => AddStickyNoteAtPos(new Point(40 + _canvas.Children.Count * 24 % 500, 40));
		private void AddStickyNoteAt()  => AddStickyNoteAtPos(_contextMenuPos);

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
				var tl = TopLevel.GetTopLevel(this); if (tl == null) return;
				var files = await tl.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
				{
					Title = "Select Image", AllowMultiple = false,
					FileTypeFilter = new[]
					{
						new FilePickerFileType("Images") { Patterns = new[] { "*.png","*.jpg","*.jpeg","*.gif","*.bmp","*.webp" } }
					}
				});
				if (files.Count == 0) return;
				var path = files[0].Path.LocalPath;
				var img  = new ImageObject(Path.GetFileName(path), path);
				img.UpdatePos(_contextMenuPos == default ? new Point(60,60) : _contextMenuPos);
				img.SetWidth(240); img.SetHeight(200);
				_mouseManager.NotifyNewItemCreated(img);
			}
			catch (Exception ex) { Console.WriteLine($"Image picker error: {ex.Message}"); }
		}

		private async void AddUrlEmbed()
		{
			var popup = new Window
			{
				Title = "Add URL / Embed", Width = 440, Height = 220,
				Background = new SolidColorBrush(Color.Parse("#2D2D3D")),
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			};
			var urlBox = new TextBox
			{
				Watermark = "https://youtube.com/watch?v=... or any URL", FontSize = 14,
				Foreground = Brushes.White, Background = new SolidColorBrush(Color.Parse("#3D3D50")),
				Margin = new Thickness(16, 12, 16, 8)
			};
			var titleBox = new TextBox
			{
				Watermark = "Display title (optional)", FontSize = 14,
				Foreground = Brushes.White, Background = new SolidColorBrush(Color.Parse("#3D3D50")),
				Margin = new Thickness(16, 0, 16, 12)
			};
			var add = new Button
			{
				Content = "Add Embed", FontSize = 14, FontWeight = FontWeight.SemiBold,
				Foreground = Brushes.White, Background = new SolidColorBrush(Color.Parse("#5555CC")),
				Margin = new Thickness(16,0,16,16), Padding = new Thickness(12,8),
				CornerRadius = new CornerRadius(8), HorizontalAlignment = HorizontalAlignment.Stretch
			};
			add.Click += (_, _) =>
			{
				var url = urlBox.Text?.Trim() ?? "";
				var ttl = titleBox.Text?.Trim() ?? "";
				if (!string.IsNullOrEmpty(url))
				{
					var embed = new UrlEmbed(url, string.IsNullOrEmpty(ttl) ? url : ttl);
					embed.UpdatePos(_contextMenuPos == default ? new Point(80,80) : _contextMenuPos);
					_mouseManager.NotifyNewItemCreated(embed);
				}
				popup.Close();
			};
			var pnl = new StackPanel();
			pnl.Children.Add(new TextBlock { Text = "Paste a URL — YouTube shows as video embed:", Foreground = Brushes.LightGray, FontSize = 12, Margin = new Thickness(16,12,16,4) });
			pnl.Children.Add(urlBox); pnl.Children.Add(titleBox); pnl.Children.Add(add);
			popup.Content = pnl;
			await popup.ShowDialog(TopLevel.GetTopLevel(this) as Window ?? throw new Exception());
		}

		private async void AddAudioFile()
		{
			try
			{
				var tl = TopLevel.GetTopLevel(this); if (tl == null) return;
				var files = await tl.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
				{
					Title = "Select Audio File", AllowMultiple = false,
					FileTypeFilter = new[] { new FilePickerFileType("Audio") { Patterns = new[] { "*.mp3","*.wav","*.ogg","*.flac","*.aac","*.m4a" } } }
				});
				if (files.Count == 0) return;
				var audio = new AudioItem(Path.GetFileName(files[0].Path.LocalPath), files[0].Path.LocalPath);
				audio.UpdatePos(_contextMenuPos == default ? new Point(80,80) : _contextMenuPos);
				_mouseManager.NotifyNewItemCreated(audio);
			}
			catch (Exception ex) { Console.WriteLine($"Audio picker error: {ex.Message}"); }
		}

		private async void AddVideoFile()
		{
			try
			{
				var tl = TopLevel.GetTopLevel(this); if (tl == null) return;
				var files = await tl.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
				{
					Title = "Select Video File", AllowMultiple = false,
					FileTypeFilter = new[] { new FilePickerFileType("Video") { Patterns = new[] { "*.mp4","*.mkv","*.avi","*.mov","*.webm","*.wmv" } } }
				});
				if (files.Count == 0) return;
				var vid = new VideoItem(Path.GetFileName(files[0].Path.LocalPath), files[0].Path.LocalPath);
				vid.UpdatePos(_contextMenuPos == default ? new Point(80,80) : _contextMenuPos);
				_mouseManager.NotifyNewItemCreated(vid);
			}
			catch (Exception ex) { Console.WriteLine($"Video picker error: {ex.Message}"); }
		}

		private void ClearAll()
		{
			var toRemove = _canvas.Children
				.OfType<Control>()
				.Where(c => c is not GridBackground)
				.ToList();
			foreach (var c in toRemove) _canvas.Children.Remove(c);
			_project.GetLstItemProject().Clear();
			MainWindow.SCtrl!.SaveProject();
		}

		// ─────────────────────────────────────────────────────────────────
		//  ITEM LIFECYCLE
		// ─────────────────────────────────────────────────────────────────

		private void LoadExistingItems()
		{
			foreach (var item in _project.GetLstItemProject())
				RenderItem(item);
		}

		private void OnItemAddedFromManager(BoardItem item)
		{
			_project.AddItem(item);
			RenderItem(item);
			MainWindow.SCtrl!.SaveProject();
		}

		private void RemoveItem(BoardItem item, Control visual)
		{
			_canvas.Children.Remove(visual);
			_project.RemoveItem(item);
			MainWindow.SCtrl!.SaveProject();
		}

		private void RenderItem(BoardItem item)
		{
			if (item == null) return;

			ItemControlBase? visual = item switch
			{
				StickyNote  note  => new StickyNoteControl { Item = note },
				ImageObject img   => new ImageControl(img),
				UrlEmbed    url   => new UrlEmbedControl(url),
				AudioItem   audio => new AudioControl(audio),
				VideoItem   vid   => new VideoControl(vid),
				DrawItem    draw  => new DrawControl(draw),
				_ => null
			};

			if (visual == null)
			{
				// Fallback for unknown types
				var fb = new Border
				{
					Child = new TextBlock
					{
						Text       = $"[Unsupported: {item.GetType().Name}]",
						Foreground = Brushes.Red
					},
					Background = Brushes.LightPink,
					Padding    = new Thickness(8)
				};
				Canvas.SetLeft(fb, item.Position.X);
				Canvas.SetTop(fb, item.Position.Y);
				_canvas.Children.Add(fb);
				return;
			}

			// Wire delete
			visual.DeleteRequested += ctrl => RemoveItem(item, ctrl);

			Canvas.SetLeft(visual, item.Position.X);
			Canvas.SetTop(visual, item.Position.Y);
			_canvas.Children.Add(visual);
		}

		protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
		{
			base.OnDetachedFromVisualTree(e);
			_mouseManager.ItemAdded -= OnItemAddedFromManager;
		}
	}

	// ─────────────────────────────────────────────────────────────────────
	//  Live preview control while drawing shapes
	// ─────────────────────────────────────────────────────────────────────

	internal class DrawPreviewControl : Control
	{
		private Point    _start;
		private Point    _end;
		private ToolMode _mode;
		private readonly Pen _pen;

		public DrawPreviewControl(Point start, ToolMode mode, string color, double strokeWidth)
		{
			_start = start;
			_end   = start;
			_mode  = mode;
			_pen   = new Pen(
				new SolidColorBrush(Color.Parse(color)),
				strokeWidth,
				dashStyle: DashStyle.Dash);

			Width  = 3000;
			Height = 3000;
			IsHitTestVisible = false;
		}

		public void UpdateEnd(Point end)
		{
			_end = end;
			InvalidateVisual();
		}

		public override void Render(DrawingContext ctx)
		{
			base.Render(ctx);
			switch (_mode)
			{
				case ToolMode.DrawLine:
					ctx.DrawLine(_pen, _start, _end);
					break;
				case ToolMode.DrawRect:
					ctx.DrawRectangle(null, _pen,
						new Rect(
							Math.Min(_start.X, _end.X), Math.Min(_start.Y, _end.Y),
							Math.Abs(_end.X - _start.X), Math.Abs(_end.Y - _start.Y)));
					break;
				case ToolMode.DrawEllipse:
					double cx = (_start.X + _end.X) / 2;
					double cy = (_start.Y + _end.Y) / 2;
					double rx = Math.Abs(_end.X - _start.X) / 2;
					double ry = Math.Abs(_end.Y - _start.Y) / 2;
					ctx.DrawEllipse(null, _pen, new Point(cx, cy), rx, ry);
					break;
			}
		}
	}

	// ─────────────────────────────────────────────────────────────────────
	//  Dot-grid background
	// ─────────────────────────────────────────────────────────────────────

	internal class GridBackground : Control
	{
		public override void Render(DrawingContext ctx)
		{
			base.Render(ctx);
			var dotBrush = new SolidColorBrush(Color.FromArgb(40, 120, 120, 180));
			const double step = 40;
			for (double x = 0; x < Bounds.Width;  x += step)
			for (double y = 0; y < Bounds.Height; y += step)
				ctx.DrawEllipse(dotBrush, null, new Point(x, y), 1.5, 1.5);
		}
	}
}
