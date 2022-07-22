using TasksApi.Entities;

namespace TasksApi.Responses
{
	public class SaveEventResponse : BaseResponse
	{
		public Event Event { get; set; }
	}
}
