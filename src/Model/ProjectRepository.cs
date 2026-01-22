using System.Text.Json;

using Model.Containers;

namespace Model
{
	// TODO: Choose Bitween ID search or Project search
	/**
	 * Project Repository :
	 * Handles Serializing and Deserializing
	 * Loading and saving projects
	 * called mainly by Controller
	 */
	public class ProjectRepository
	{
		private static List<Project> s_listProject = new List<Project>();
	
		public static Project? LoadProject(int id)
		{
			if (id >= 0 && id < ProjectRepository.s_listProject.Count)
				return ProjectRepository.s_listProject[id];

			return null;
		}

		public static Boolean SaveProject(Project p)
		{
			if (!ProjectRepository.s_listProject.Contains(p))
			{
				ProjectRepository.s_listProject.Add(p);
				return true;
			}

			return false;
		}

		public static Boolean Serialize()
		{
			var options = new JsonSerializerOptions();
			options.WriteIndented = true;

			string jsonString = JsonSerializer.Serialize(ProjectRepository.s_listProject, options);
			
			Console.WriteLine(jsonString);
			
			File.WriteAllText("Projects.json", jsonString);
			
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Success in saving to json");
			Console.ResetColor();

			return true;
		}
		public static Boolean Deserialize()
		{
			if (!File.Exists("Projects.json")) return false;
			var projectsJson = File.ReadAllText("Projects.json");
			if (projectsJson == null) return false;

			ProjectRepository.s_listProject = JsonSerializer.Deserialize<List<Project>>(projectsJson);
			return true;
		}

		public static List<Project> GetLstProject()
		{
			return ProjectRepository.s_listProject;
		}

		public static Boolean DeleteProject(Project p)
		{
			return ProjectRepository.s_listProject.Remove(p);
		}
	}
}
