using TasksApi.Entities;

namespace TasksApi.Responses
{
	public class GetMembersResponse :BaseResponse
	{
		public List<Member> Members { get; set; }
	}
}
