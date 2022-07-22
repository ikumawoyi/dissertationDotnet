using System.Text.Json.Serialization;

namespace TasksApi.Responses
{
	public class DeleteOccasionResponse : BaseResponse
	{
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public int OccasionId { get; set; }
	}
}
