using EmailService;
using Hangfire;
using TasksApi.Entities;
using TasksApi.Interfaces;
using TasksApi.Responses;

namespace TasksApi.Services
{
	public class MemberService : IMemberService
    {
        private readonly TasksDbContext tasksDbContext;

        private readonly IEmailSender _emailSender;

        public MemberService(TasksDbContext tasksDbContext, IEmailSender emailSender)
		{
			this.tasksDbContext = tasksDbContext;
			_emailSender = emailSender;
		}

		public async Task<DeleteMemberResponse> DeleteMember(int memberId)
		{
            var member = await tasksDbContext.Members.FindAsync(memberId);

            if (member == null)
            {
                return new DeleteMemberResponse
                {
                    Success = false,
                    Error = "Task not found",
                    ErrorCode = "T01"
                };
            }

            tasksDbContext.Members.Remove(member);

            var saveResponse = await tasksDbContext.SaveChangesAsync();

            if (saveResponse >= 0)
            {
                return new DeleteMemberResponse
                {
                    Success = true,
                    MemberId = member.Id
                };
            }

            return new DeleteMemberResponse
            {
                Success = false,
                Error = "Unable to delete task",
                ErrorCode = "T03"
            };
        }

		public IEnumerable<Member> GetMembers()
		{
			var members  = tasksDbContext.Members;
            return members;
		}

        public void SendEmail()
        {
            // I need to include logic to get event date, check with current date, if the condition returns true, then i send mail to the address.
            // Dictionary<string, string> data = new Dictionary<string, string>();
            var dateofBirth = tasksDbContext.Members.Where(x => x.DateOfBirth == DateTime.Now.AddHours(5)).ToList();
            if (dateofBirth.Count != 0)
            {
                foreach (var item in dateofBirth)
                {
                    var Birthdaymessage = new Message(item.Email, "Test email", "This is to wish you a happy birthday from CIA");
                    _emailSender.SendEmail(Birthdaymessage);
                };
            }
        }

        public async Task<SaveMemberResponse> SaveMember(Member member)
		{
            if (member.Id == 0)
            {
                await tasksDbContext.Members.AddAsync(member);
            }
            else
            {
                var memberRecord = await tasksDbContext.Members.FindAsync(member.Id);

                memberRecord.Name = member.Name;
                memberRecord.Email = member.Email;
                memberRecord.PhoneNumber = member.PhoneNumber;
                memberRecord.Address = member.Address;
                memberRecord.DateOfBirth = member.DateOfBirth;
                memberRecord.Ts = DateTime.Now;
            }

            var saveResponse = await tasksDbContext.SaveChangesAsync();

            if (saveResponse >= 0)
            {
                return new SaveMemberResponse
                {
                    Success = true,
                    Member = member
                };
            }
            return new SaveMemberResponse
            {
                Success = false,
                Error = "Unable to save task",
                ErrorCode = "T05"
            };
        }

		public Member GetMember(int memberId)
		{
            var member = tasksDbContext.Members.FirstOrDefault(x => x.Id == memberId);
            return member;
        }
	}
}
