// Avalonia imports
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;

//Model
using Model.Containers;
using Model.Geometry;
using Model.Items;

//View
using View;
using View.Objects;
using View.Systems;

namespace View.Panels
{
	public class MindBoardPanel : DockPanel
	{
		private readonly MouseManager _mouseManager;
		private readonly Project _project;

		private Canvas _canvas = null!;
		private StackPanel _toolbar = null!;

		public MindBoardPanel(Project project, MouseManager mouseManager)
		{
			_project = project ?? throw new ArgumentNullException(nameof(project));
			_mouseManager = mouseManager ?? throw new ArgumentNullException(nameof(mouseManager));

			Background = SystemStyle.Background;

			MainWindow.SCtrl.SetProject(project);
			
			var title = new TextBlock
			{
				Text = $"Mind Board: {_project.GetName()}",
				FontSize = 24,
				FontWeight = FontWeight.Bold,
				Margin = new Thickness(16, 12, 16, 8),
				HorizontalAlignment = HorizontalAlignment.Center
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

			_canvas = new Canvas
			{
				Background = Brushes.Transparent,
				ClipToBounds = false
			};
			Grid.SetColumn(_canvas, 1);
			mainGrid.Children.Add(_canvas);

			Children.Add(mainGrid);

			_mouseManager.ItemAdded += OnItemAddedFromManager;

			LoadExistingItems();
		}

		private StackPanel CreateLeftToolbar()
		{
			var panel = new StackPanel
			{
				Margin = new Thickness(16),
				Spacing = 16,
				Orientation = Orientation.Vertical,
				Background = SystemStyle.TopBanner
			};

			var header = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = 12,
				Margin = new Thickness(0, 0, 0, 20)
			};

			var icon = new Image
			{
				Source = new Bitmap("Icons/mindmap.png"),
				Width = 36,
				Height = 36
			};

			var name = new TextBlock
			{
				Text = _project.GetName() ?? "Untitled Project",
				FontSize = 18,
				FontWeight = FontWeight.SemiBold,
				VerticalAlignment = VerticalAlignment.Center
			};

			header.Children.Add(icon);
			header.Children.Add(name);
			panel.Children.Add(header);

			var btnSticky = new Button
			{
				Content = "Add Sticky Note",
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Padding = new Thickness(12, 10),
				Margin = new Thickness(0, 4)
			};
			btnSticky.Click += AddStickyNote_Click;
			panel.Children.Add(btnSticky);


			return panel;
		}

		private async void AddStickyNote_Click(object? sender, RoutedEventArgs e)
		{
			string defaultText = "This is a temporary note to test things out.";

			var note = new StickyNote(defaultText);

			double offset = 40 + _canvas.Children.Count * 25;
			note.UpdatePos(new Point(offset, offset));

			_mouseManager.NotifyNewItemCreated(note);
		}

		private void LoadExistingItems()
		{
			foreach (var item in _project.GetLstItemProject())
			{
				RenderItem(item);
			}
		}

		private void OnItemAddedFromManager(BoardItem item)
		{
			_project.AddItem(item);
			RenderItem(item);
			Console.WriteLine("Saving Item ADDED");
			MainWindow.SCtrl.SaveProject();
		}

		private void RenderItem(BoardItem item)
		{
			if (item == null) return;

			Control? visual = item switch
			{
				StickyNote note => new StickyNoteControl
				{
					Item = note
				},


				_ => new Border
				{
					Child = new TextBlock { Text = $"[Unsupported: {item.GetType().Name}]", Foreground = Brushes.Red },
					Background = Brushes.LightPink,
					Padding = new Thickness(8)
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
}
