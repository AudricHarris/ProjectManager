using System.Globalization;
using System.Text.Json.Serialization;
using Avalonia;

namespace Model.Items
{
	public enum DrawShape { Freehand, Line, Rectangle, Ellipse }

	public class DrawItem : BoardItem
	{
		public DrawShape Shape       { get; set; }
		public string    Color       { get; set; } = "#FFFFFF";
		public double    StrokeWidth { get; set; } = 2;
		public double    Rotation    { get; set; }

		/// <summary>
		/// Points stored as offsets relative to (PosX, PosY).
		/// This means dragging only updates PosX/PosY; points never change.
		/// </summary>
		public List<string> PointData { get; set; } = new();

		/// <summary>Points in local (relative) coordinates.</summary>
		[JsonIgnore]
		public List<Point> RelativePoints
		{
			get => PointData.Select(s =>
			{
				var parts = s.Split(',');
				return new Point(
					double.Parse(parts[0], CultureInfo.InvariantCulture),
					double.Parse(parts[1], CultureInfo.InvariantCulture));
			}).ToList();
			set => PointData = value
				.Select(p => FormattableString.Invariant($"{p.X},{p.Y}"))
				.ToList();
		}

		public DrawItem() { }

		/// <summary>
		/// Construct from absolute canvas points.
		/// Computes bounding-box origin, stores as PosX/PosY, stores points as relative offsets.
		/// </summary>
		public DrawItem(DrawShape shape, List<Point> absPoints, string color, double strokeWidth)
			: base(0, new Point(), 0, 0, 0)
		{
			Shape       = shape;
			Color       = color;
			StrokeWidth = strokeWidth;

			if (absPoints.Count == 0) return;

			double pad  = strokeWidth + 2;
			double minX = absPoints.Min(p => p.X) - pad;
			double minY = absPoints.Min(p => p.Y) - pad;
			double maxX = absPoints.Max(p => p.X) + pad;
			double maxY = absPoints.Max(p => p.Y) + pad;

			PosX   = minX;
			PosY   = minY;
			Width  = (int)Math.Ceiling(maxX - minX);
			Height = (int)Math.Ceiling(maxY - minY);

			// Store points relative to bounding-box origin
			RelativePoints = absPoints.Select(p => new Point(p.X - minX, p.Y - minY)).ToList();
		}

		public override string ToString() => $"\t-[Draw] {Shape}\n";
	}
}
