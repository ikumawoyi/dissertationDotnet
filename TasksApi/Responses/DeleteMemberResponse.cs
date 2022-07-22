using System.Text.Json.Serialization;

namespace TasksApi.Responses
{
	public class DeleteMemberResponse : BaseResponse
	{
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public int MemberId { get; set; }
	}
}
