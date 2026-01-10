using System.Text.Json;

using Model.Containers;

namespace Model
{
	// TODO: Choose Bitween ID search or Project search
	public class	ProjectRepository
	{
		private static List<Project> listProject = new List<Project>();
	
		public static Project? loadProject(int id)
		{
			if (id >= 0 && id < ProjectRepository.listProject.Count)
				return ProjectRepository.listProject[id];

			return null;
		}

		public static Boolean saveProject(Project p)
		{
			if (!ProjectRepository.listProject.Contains(p))
			{
				ProjectRepository.listProject.Add(p);
				return true;
			}

			return false;
		}

		public static Boolean serialize()
		{
			var options = new JsonSerializerOptions();
			options.WriteIndented = true;

			string jsonString = JsonSerializer.Serialize(ProjectRepository.listProject, options);
			
			Console.WriteLine(jsonString);
			
			File.WriteAllText("Projects.json", jsonString);
			
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Success in saving to json");
			Console.ResetColor();

			return true;
		}
		public static Boolean deserialize()
		{
			var projectsJson = File.ReadAllText("Projects.json");

			ProjectRepository.listProject = JsonSerializer.Deserialize<List<Project>>(projectsJson);
			return true;
		}

		public static List<Project> getLstProject()
		{
			return ProjectRepository.listProject;
		}

		public static Boolean deleteProject(int id)
		{
			if (id >= 0 && id < ProjectRepository.listProject.Count)
			{
				ProjectRepository.listProject.RemoveAt(id);
				return true;
			}

			return false;
		}
	}
}
