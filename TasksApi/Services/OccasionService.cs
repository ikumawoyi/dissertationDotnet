using EmailService;
using Hangfire;
using TasksApi.Entities;
using TasksApi.Interfaces;
using TasksApi.Responses;

namespace TasksApi.Services
{
	public class OccasionService : IOccasionService
    {
        private readonly TasksDbContext tasksDbContext;

        private readonly IEmailSender _emailSender;

        public OccasionService(TasksDbContext tasksDbContext, IEmailSender emailSender)
		{
			this.tasksDbContext = tasksDbContext;
			_emailSender = emailSender;
		}

		public async Task<DeleteOccasionResponse> DeleteOccasion(int occasionId)
		{
            var occasion = await tasksDbContext.Occasions.FindAsync(occasionId);

            if (occasion == null)
            {
                return new DeleteOccasionResponse
                {
                    Success = false,
                    Error = "Task not found",
                    ErrorCode = "T01"
                };
            }

            tasksDbContext.Occasions.Remove(occasion);

            var saveResponse = await tasksDbContext.SaveChangesAsync();

            if (saveResponse >= 0)
            {
                return new DeleteOccasionResponse
                {
                    Success = true,
                    OccasionId = occasion.Id
                };
            }

            return new DeleteOccasionResponse
            {
                Success = false,
                Error = "Unable to delete task",
                ErrorCode = "T03"
            };
        }

		public IEnumerable<Occasion> GetOccasions()
		{
			var occasions = tasksDbContext.Occasions;
            return occasions;
		}

        public void SendEmail()
        {
            // I need to include logic to get event date, check with current date, if the condition returns true, then i send mail to the address.
            // Dictionary<string, string> data = new Dictionary<string, string>();
            //var events = tasksDbContext.Events.Where(x => x.EventTime.AddDays(-1) == DateTime.Now).ToList();

            var occasions = tasksDbContext.Occasions.Where(x => x.StartDate.AddDays(-1) == DateTime.Now).ToList();
            if (occasions.Count != 0)
            {
                foreach (var item in occasions)
                {
                    //var achorsEmail = item.EventAnchors.Split(',');

                    //  Construct the content of the mail

                    //var cont = "This is to remind you about" { item.EventName}
                    var Anchormessage = new Message(item.Anchors, "Test email", "This is to remind you about");
                    _emailSender.SendEmail(Anchormessage);
                    var Attendeemessage = new Message(item.Attendees, "Test email", "This is the mail from church scheduler app.");
                    _emailSender.SendEmail(Attendeemessage);

                    //var message = new Message("platinny10@gmail.com,yemibutwhy@gmail.com,bolu2009@yahoo.com", "Test email", "This is the mail from church scheduler app.");
                    //_emailSender.SendEmail(message);
                    //return $"Mail sent from Hangfire";
                };
            }
        }


        public async Task<SaveOccasionResponse> SaveOccasion(Occasion occasion)
		{
            if (occasion.Id == 0)
            {
                await tasksDbContext.Occasions.AddAsync(occasion);
            }
            else
            {
                var occasionRecord = await tasksDbContext.Occasions.FindAsync(occasion.Id);

                occasionRecord.Name = occasion.Name;
                occasionRecord.Anchors = occasion.Anchors;
                occasionRecord.Attendees = occasion.Attendees;
                occasionRecord.StartDate = occasion.StartDate;
                occasionRecord.Ts = DateTime.Now;
            }

            var saveResponse = await tasksDbContext.SaveChangesAsync();

            if (saveResponse >= 0)
            {
                return new SaveOccasionResponse
                {
                    Success = true,
                    Occasion = occasion
                };
            }
            return new SaveOccasionResponse
            {
                Success = false,
                Error = "Unable to save task",
                ErrorCode = "T05"
            };
        }

		public Occasion GetOccasion(int occasionId)
		{
            var occasion = tasksDbContext.Occasions.FirstOrDefault(x => x.Id == occasionId);
            return occasion;
        }
	}
}
