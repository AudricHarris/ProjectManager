using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using Model.Items;

namespace View.Objects
{
	public class DrawControl : ItemControlBase
	{
		public DrawItem Item { get; private set; }

		public DrawControl(DrawItem item)
		{
			Item = item;

			Width  = Math.Max(item.Width,  10);
			Height = Math.Max(item.Height, 10);

			// Place at saved position (works both on first create and on reload after drag)
			Canvas.SetLeft(this, item.PosX);
			Canvas.SetTop(this,  item.PosY);

			// Inner renderer uses relative points directly — no offset math needed
			Children.Add(new ShapeCanvas(item, Width, Height));

			Cursor = new Cursor(StandardCursorType.SizeAll);
			InitHandles(item.Rotation);
		}

		protected override void OnPositionChanged(Point p) => Item.UpdatePos(p);
		protected override void OnRotationChanged(double deg) => Item.Rotation = deg;
	}

	internal class ShapeCanvas : Control
	{
		private readonly DrawItem _item;

		public ShapeCanvas(DrawItem item, double w, double h)
		{
			_item  = item;
			Width  = w;
			Height = h;
		}

		public override void Render(DrawingContext ctx)
		{
			base.Render(ctx);

			var pts = _item.RelativePoints;
			if (pts.Count == 0) return;

			var pen = new Pen(
				new SolidColorBrush(Color.Parse(_item.Color)),
				_item.StrokeWidth,
				lineCap: PenLineCap.Round,
				lineJoin: PenLineJoin.Round);

			switch (_item.Shape)
			{
				case DrawShape.Freehand:
					if (pts.Count < 2) break;
					var geo = new StreamGeometry();
					using (var gctx = geo.Open())
					{
						gctx.BeginFigure(pts[0], false);
						for (int i = 1; i < pts.Count; i++)
							gctx.LineTo(pts[i]);
					}
					ctx.DrawGeometry(null, pen, geo);
					break;

				case DrawShape.Line:
					ctx.DrawLine(pen, pts[0], pts[^1]);
					break;

				case DrawShape.Rectangle:
					ctx.DrawRectangle(null, pen,
						new Rect(
							Math.Min(pts[0].X, pts[^1].X),
							Math.Min(pts[0].Y, pts[^1].Y),
							Math.Abs(pts[^1].X - pts[0].X),
							Math.Abs(pts[^1].Y - pts[0].Y)));
					break;

				case DrawShape.Ellipse:
					ctx.DrawEllipse(null, pen,
						new Point((pts[0].X + pts[^1].X) / 2, (pts[0].Y + pts[^1].Y) / 2),
						Math.Abs(pts[^1].X - pts[0].X) / 2,
						Math.Abs(pts[^1].Y - pts[0].Y) / 2);
					break;
			}
		}
	}
}
