using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Layout;

namespace View.Objects
{
	/// <summary>
	/// Base canvas for all board items.
	/// - Click to select (shows handles + highlight border)
	/// - Click elsewhere to deselect
	/// - Drag to move (only when selected)
	/// - Rotate handle: top-center of the card
	/// - Delete handle: top-right corner of the card (inside bounds)
	/// </summary>
	public abstract class ItemControlBase : Canvas
	{
		// ── Selection ─────────────────────────────────────────────────────
		private bool _selected;
		private Border? _selectionBorder;   // selection highlight overlay

		// ── Drag ──────────────────────────────────────────────────────────
		private bool  _dragging;
		private Point _dragOffset;

		// ── Rotate ────────────────────────────────────────────────────────
		private bool   _rotating;
		private double _rotateStartAngle;
		private Point  _rotateStartMouse;

		// ── Angle ─────────────────────────────────────────────────────────
		private double _angleDeg;
		public  double AngleDeg
		{
			get => _angleDeg;
			set
			{
				_angleDeg             = value;
				RenderTransform       = new RotateTransform(value);
				RenderTransformOrigin = RelativePoint.Center;
			}
		}

		public event Action<ItemControlBase>? DeleteRequested;

		// ── Handle controls ───────────────────────────────────────────────
		private Border? _rotateHandle;
		private Border? _deleteHandle;

		// ── Static deselect: only one item selected at a time ─────────────
		private static ItemControlBase? _currentSelected;

		// ── Init ──────────────────────────────────────────────────────────
		protected void InitHandles(double initAngle = 0)
		{
			// Selection border overlay — sits on top of everything inside the card
			_selectionBorder = new Border
			{
				IsHitTestVisible = false,
				BorderBrush      = new SolidColorBrush(Color.Parse("#5599FF")),
				BorderThickness  = new Thickness(2),
				CornerRadius     = new CornerRadius(10),
				IsVisible        = false,
				ZIndex           = 50
			};
			Children.Add(_selectionBorder);

			_rotateHandle = MakeRotateHandle();
			_deleteHandle = MakeDeleteHandle();
			Children.Add(_rotateHandle);
			Children.Add(_deleteHandle);

			SetHandlesVisible(false);
			AngleDeg = initAngle;

			SizeChanged    += (_, _) => PositionHandles();
			LayoutUpdated  += (_, _) => PositionHandles();

			PointerPressed  += OnBasePointerPressed;
			PointerMoved    += OnDragMoved;
			PointerReleased += OnDragReleased;
		}

		// ── Handle layout — INSIDE the card bounds ────────────────────────
		private void PositionHandles()
		{
			if (_selectionBorder != null)
			{
				_selectionBorder.Width  = Bounds.Width;
				_selectionBorder.Height = Bounds.Height;
				Canvas.SetLeft(_selectionBorder, 0);
				Canvas.SetTop(_selectionBorder,  0);
			}

			if (_rotateHandle != null)
			{
				// Top-center, slightly inside the top edge
				Canvas.SetLeft(_rotateHandle, Bounds.Width / 2 - _rotateHandle.Width / 2);
				Canvas.SetTop(_rotateHandle,  4);
			}

			if (_deleteHandle != null)
			{
				// Top-right corner, slightly inside
				Canvas.SetLeft(_deleteHandle, Bounds.Width - _deleteHandle.Width - 4);
				Canvas.SetTop(_deleteHandle,  4);
			}
		}

		private Border MakeRotateHandle()
		{
			var b = new Border
			{
				Width           = 22,
				Height          = 22,
				Background      = new SolidColorBrush(Color.Parse("#3366DD")),
				BorderBrush     = Brushes.White,
				BorderThickness = new Thickness(1.5),
				CornerRadius    = new CornerRadius(11),
				Cursor          = new Cursor(StandardCursorType.Cross),
				ZIndex          = 200,
				Child = new TextBlock
				{
					Text      = "↻",
					FontSize  = 13,
					Foreground = Brushes.White,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment   = VerticalAlignment.Center
				}
			};
			b.PointerPressed  += RotateHandle_Pressed;
			b.PointerMoved    += RotateHandle_Moved;
			b.PointerReleased += RotateHandle_Released;
			return b;
		}

		private Border MakeDeleteHandle()
		{
			var b = new Border
			{
				Width           = 22,
				Height          = 22,
				Background      = new SolidColorBrush(Color.Parse("#CC2222")),
				BorderBrush     = Brushes.White,
				BorderThickness = new Thickness(1.5),
				CornerRadius    = new CornerRadius(11),
				Cursor          = new Cursor(StandardCursorType.Arrow),
				ZIndex          = 200,
				Child = new TextBlock
				{
					Text      = "✕",
					FontSize  = 12,
					Foreground = Brushes.White,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment   = VerticalAlignment.Center
				}
			};
			b.PointerPressed += (_, e) =>
			{
				e.Handled = true;
				DeleteRequested?.Invoke(this);
			};
			return b;
		}

		private void SetHandlesVisible(bool visible)
		{
			if (_rotateHandle    != null) _rotateHandle.IsVisible    = visible;
			if (_deleteHandle    != null) _deleteHandle.IsVisible    = visible;
			if (_selectionBorder != null) _selectionBorder.IsVisible = visible;
		}

		// ── Selection ─────────────────────────────────────────────────────
		public void Select()
		{
			if (_selected) return;

			// Deselect previous
			if (_currentSelected != null && _currentSelected != this)
				_currentSelected.Deselect();

			_selected        = true;
			_currentSelected = this;
			SetHandlesVisible(true);
			ZIndex = 999;
		}

		public void Deselect()
		{
			if (!_selected) return;
			_selected = false;
			if (_currentSelected == this) _currentSelected = null;
			SetHandlesVisible(false);
			ZIndex = 0;
		}

		// ── Pointer input ─────────────────────────────────────────────────
		private void OnBasePointerPressed(object? sender, PointerPressedEventArgs e)
		{
			// Don't handle right-click
			if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed) return;

			// If click lands on a handle, let the handle handle it
			if (IsHandleSource(e.Source)) return;

			if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
			{
				Select();

				_dragging   = true;
				_dragOffset = e.GetPosition(this);
				e.Pointer.Capture(this);
				e.Handled = true;
			}
		}

		private bool IsHandleSource(object? src)
		{
			if (src == _rotateHandle || src == _deleteHandle) return true;
			if (src is Control c && (c.Parent == _rotateHandle || c.Parent == _deleteHandle)) return true;
			return false;
		}

		private void OnDragMoved(object? sender, PointerEventArgs e)
		{
			if (!_dragging || Parent is not Canvas canvas) return;
			var    pos = e.GetPosition(canvas);
			double x   = pos.X - _dragOffset.X;
			double y   = pos.Y - _dragOffset.Y;
			Canvas.SetLeft(this, x);
			Canvas.SetTop(this,  y);
			OnPositionChanged(new Point(x, y));
			e.Handled = true;
		}

		private void OnDragReleased(object? sender, PointerReleasedEventArgs e)
		{
			if (_dragging) { _dragging = false; e.Pointer.Capture(null); }
		}

		protected virtual void OnPositionChanged(Point p) { }

		// ── Rotate ────────────────────────────────────────────────────────
		private void RotateHandle_Pressed(object? sender, PointerPressedEventArgs e)
		{
			if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
			_rotating         = true;
			_rotateStartMouse = e.GetPosition(Parent as Visual);
			_rotateStartAngle = _angleDeg;
			e.Pointer.Capture(_rotateHandle as IInputElement);
			e.Handled = true;
		}

		private void RotateHandle_Moved(object? sender, PointerEventArgs e)
		{
			if (!_rotating || Parent is not Canvas canvas) return;

			var    mouse   = e.GetPosition(canvas);
			double cx      = Canvas.GetLeft(this) + Bounds.Width  / 2;
			double cy      = Canvas.GetTop(this)  + Bounds.Height / 2;
			double dx1     = _rotateStartMouse.X - cx;
			double dy1     = _rotateStartMouse.Y - cy;
			double dx2     = mouse.X - cx;
			double dy2     = mouse.Y - cy;
			double angle1  = Math.Atan2(dy1, dx1) * 180 / Math.PI;
			double angle2  = Math.Atan2(dy2, dx2) * 180 / Math.PI;
			AngleDeg       = _rotateStartAngle + (angle2 - angle1);
			OnRotationChanged(AngleDeg);
			e.Handled = true;
		}

		private void RotateHandle_Released(object? sender, PointerReleasedEventArgs e)
		{
			if (_rotating) { _rotating = false; e.Pointer.Capture(null); }
		}

		protected virtual void OnRotationChanged(double deg) { }

		// ── Global deselect when canvas background clicked ─────────────────
		public static void DeselectAll()
		{
			_currentSelected?.Deselect();
		}
	}
}
