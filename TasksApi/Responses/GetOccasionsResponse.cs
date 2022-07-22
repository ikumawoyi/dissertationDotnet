using TasksApi.Entities;

namespace TasksApi.Responses
{
	public class GetOccasionsResponse :BaseResponse
	{
		public List<Member> Occasions { get; set; }
	}
}
