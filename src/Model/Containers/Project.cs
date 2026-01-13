using Model.Items;

namespace Model.Containers
{
	/**
	 * Project : 
	 * Each project has Defining traits {id, name, desc, date}
	 * Projects store items and connections for the mind map
	 * Each Project can be visualized as a mid map
	 */
	public class Project
	{
		private static int nbProject = 0;

		public int Id { get; set; }
		public string Name { get; set; }
		public string Desc { get; set; } 
		public DateTime DateCreation { get; set; }
		
		public List<BoardItem> LstItemProject {get; set; }

		//----------------------//
		//    Constructors      //
		//----------------------//

		public Project(string name, string desc, DateTime dateCreation)
		{
			this.Id   = ++nbProject;
			this.Name = name;
			this.Desc = desc;
			this.DateCreation = dateCreation;

			this.LstItemProject = new List<BoardItem>();
		}

		public Project(string nom, string desc) : this(nom, desc, DateTime.Now) {}
		public Project(string nom) : this(nom, "") {}
		public Project() : this("tmp") {}

		//----------------------//
		//       SETTERS        //
		//----------------------//
		public void SetId(int id)        { this.Id = id;          }
		public void SetName(string name) { this.Name = name;      }
		public void SetDesc(string desc) { this.Desc = desc;      }
		public void SetDate(DateTime d)  { this.DateCreation = d; }

		public void SetLstItemProject(List<BoardItem> lst) { this.LstItemProject = lst; }

		//----------------------//
		//       GETTERS        //
		//----------------------//

		public int      GetId  () { return this.Id;           }
		public string   GetName() { return this.Name;         }
		public string   GetDesc() { return this.Desc;         }
		public DateTime GetDate() { return this.DateCreation; }

		public List<BoardItem> GetLstItemProject() { return this.LstItemProject; }

		//----------------------//
		//   Instance Methods   //
		//----------------------//
		

		public Boolean AddItem(BoardItem item) 
		{ 
			if (item != null && !this.LstItemProject.Contains(item)) 
			{
				this.LstItemProject.Add(item); 
				return true;
			}
			return false;
		}

		public Boolean RemoveItem(BoardItem item) 
		{
			if (item != null && this.LstItemProject.Contains(item))
			{
				this.LstItemProject.Remove(item);
				return true;
			}
			return false;
		}

		override
		public string ToString()
		{
			String res = new string('-', 105) + "\n";
			
			res += "Project\n";
			res += "\tName : " + this.Name + "\n";
			res += "\tDate [ " + this.DateCreation + " ]\n";
			res += "\tDesc : " + this.Desc + "\n";
			
			res += new string('-', 105) + "\n";
			
			res += "Items\n";
			foreach (BoardItem item in this.LstItemProject) 
				res += item.ToString();

			res += new string('-', 105);

			return res;
		}
	}
}
