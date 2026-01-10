using Model.Geometry;

namespace Model.Items
{
	// TODO: Convert images to Array of bytes for serializing
	/**
	 * Image :
	 * extends board item handles Image files
	 */
	public class Image : BoardItem
	{
		public string name { get; set; }
		public string path { get; set; }

		public Image(string name, string path) : base(0, new Point(), 0, 0, 0)
		{
			this.name = name;
			this.path = path;
		}

		//----------------------//
		//       SETTERS        //
		//----------------------//
		public void setName(string name) { this.name = name; }
		public void setPath(string path) { this.path = path; }
		
		//----------------------//
		//       GETTERS        //
		//----------------------//
		public string getName() { return this.name; }
		public string getpath() { return this.path; }

		//----------------------//
		//    Method Instance   //
		//----------------------//
		
		override
		public string ToString()
		{
			return "\t-" + this.name + "\n";
		}
	}
}
