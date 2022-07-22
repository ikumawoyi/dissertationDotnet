using TasksApi.Entities;

namespace TasksApi.Responses
{
	public class GetEventsResponse : BaseResponse
	{
		public List<Event> Events { get; set; }
	}
}
