namespace TasksApi.Entities
{
	public class Event
	{
		public int Id { get; set; }
		public string EventName { get; set; }
		public string EventAnchorsId { get; set; }
		public string EventAttendeesId { get; set; }
		//public string Message { get; set; }
		public DateTime Ts { get; set; }
		public DateTime EventTime { get; set; }
	}
}