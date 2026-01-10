using Model.Items;

namespace Model.Containers
{
	public class Project
	{
		private static int nbProject = 0;

		public int       id { get; set; }
		public string   name { get; set; }
		public string   desc { get; set; } 
		public DateTime dateCreation { get; set; }
		
		public List<BoardItem> lstItemProject {get; set; }

		//----------------------//
		//    Constructors      //
		//----------------------//

		public Project(string name, string desc, DateTime dateCreation)
		{
			this.id   = ++nbProject;
			this.name = name;
			this.desc = desc;
			this.dateCreation = dateCreation;

			this.lstItemProject = new List<BoardItem>();
		}

		public Project(string nom, string desc) : this(nom, desc, DateTime.Now) {}
		public Project(string nom) : this(nom, "") {}
		public Project() : this("tmp") {}

		//----------------------//
		//       SETTERS        //
		//----------------------//
		public void setId(int id)        { this.id = id;          }
		public void setName(string name) { this.name = name;      }
		public void setDesc(string desc) { this.desc = desc;      }
		public void setDate(DateTime d)  { this.dateCreation = d; }

		public void setLstItemProject(List<BoardItem> lst) { this.lstItemProject = lst; }

		//----------------------//
		//       GETTERS        //
		//----------------------//

		public int      getId  () { return this.id;           }
		public string   getName() { return this.name;         }
		public string   getDesc() { return this.desc;         }
		public DateTime getDate() { return this.dateCreation; }

		public List<BoardItem> getLstItemProject() { return this.lstItemProject; }

		//----------------------//
		//   Instance Methods   //
		//----------------------//
		

		public Boolean addItem(BoardItem item) 
		{ 
			if (item != null && !this.lstItemProject.Contains(item)) 
			{
				this.lstItemProject.Add(item); 
				return true;
			}
			return false;
		}

		public Boolean removeItem(BoardItem item) 
		{
			if (item != null && this.lstItemProject.Contains(item))
			{
				this.lstItemProject.Remove(item);
				return true;
			}
			return false;
		}

		override
		public string ToString()
		{
			String res = new string('-', 105) + "\n";
			
			res += "Project\n";
			res += "\tName : " + this.name + "\n";
			res += "\tDate [ " + this.dateCreation + " ]\n";
			res += "\tDesc : " + this.desc + "\n";
			
			res += new string('-', 105) + "\n";
			
			res += "Items\n";
			foreach (BoardItem item in this.lstItemProject) 
				res += item.ToString();

			res += new string('-', 105);

			return res;
		}
	}
}
