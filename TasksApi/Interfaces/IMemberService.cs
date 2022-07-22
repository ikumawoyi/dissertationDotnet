using TasksApi.Entities;
using TasksApi.Responses;

namespace TasksApi.Interfaces
{
	public interface IMemberService
	{
		void SendEmail();
		IEnumerable<Member> GetMembers();

		Task<SaveMemberResponse> SaveMember(Member member);
		Member GetMember(int memberId);

		Task<DeleteMemberResponse> DeleteMember(int memberId);
	}
}
