namespace Model.Items
{
	public struct Point
	{
		public int X { get; init; }
		public int Y { get; init; }

		// Constructors
		public Point() : this(0,0) {}

		public Point(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

    	// To String
    	public override string ToString()
    	{
        	return $"({this.X},{this.Y})";
    	}

    	// methode static

    	public static Point add(Point p1, Point p2)
    	{
    		return new Point(p1.X + p2.X, p1.Y + p2.Y); 
    	}

		public static Point sub(Point p1, Point p2)
    	{
    		return new Point(p1.X - p2.X, p1.Y - p2.Y); 
    	}
	}
}
