namespace TasksApi.Entities
{
	public class Occasion
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Anchors { get; set; }
		public string Attendees { get; set; }
		public DateTime Ts { get; set; }
		public DateTime StartDate { get; set; }
	}
}
