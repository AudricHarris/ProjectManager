using Model.Geometry;
using Avalonia;

namespace Model.Items
{
	// TODO: Convert images to Array of bytes for serializing
	/**
	 * Image :
	 * extends board item handles Image files
	 */
	public class ImageObject : BoardItem
	{
		public string Name { get; set; }
		public string Path { get; set; }

		public ImageObject(string name, string path) : base(0, new Point(), 0, 0, 0)
		{
			this.Name = name;
			this.Path = path;
		}

		//----------------------//
		//       SETTERS        //
		//----------------------//
		public void SetName(string name) { this.Name = name; }
		public void SetPath(string path) { this.Path = path; }
		
		//----------------------//
		//       GETTERS        //
		//----------------------//
		public string GetName() { return this.Name; }
		public string Getpath() { return this.Path; }

		//----------------------//
		//    Method Instance   //
		//----------------------//
		
		override
		public string ToString()
		{
			return "\t-" + this.Name + "\n";
		}
	}
}
