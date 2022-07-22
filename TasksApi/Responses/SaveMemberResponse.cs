using TasksApi.Entities;

namespace TasksApi.Responses
{
	public class SaveMemberResponse : BaseResponse
	{
		public Member Member { get; set; }
	}
}
