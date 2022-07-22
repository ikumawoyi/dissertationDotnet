using EmailService;
using TasksApi.Entities;
using TasksApi.Interfaces;
using TasksApi.Responses;

namespace TasksApi.Services
{
	public class EventService : IEventService
	{
        private readonly TasksDbContext tasksDbContext;
        private readonly IEmailSender _emailSender;

        public EventService(TasksDbContext tasksDbContext, IEmailSender emailSender)
		{
			this.tasksDbContext = tasksDbContext;
            this._emailSender = emailSender;
		}

		public async Task<DeleteEventResponse> DeleteEvent(int eventId)
		{
            var events = await tasksDbContext.Events.FindAsync(eventId);

            if (events == null)
            {
                return new DeleteEventResponse
                {
                    Success = false,
                    Error = "Task not found",
                    ErrorCode = "T01"
                };
            }

            tasksDbContext.Events.Remove(events);

            var saveResponse = await tasksDbContext.SaveChangesAsync();

            if (saveResponse >= 0)
            {
                return new DeleteEventResponse
                {
                    Success = true,
                    EventId = events.Id
                };
            }

            return new DeleteEventResponse
            {
                Success = false,
                Error = "Unable to delete task",
                ErrorCode = "T03"
            };
        }

        public void SendEmail()
        {
            // I need to include logic to get event date, check with current date, if the condition returns true, then i send mail to the address.
           // Dictionary<string, string> data = new Dictionary<string, string>();
            var events = tasksDbContext.Events.Where(x => x.EventTime.AddDays(-1) == DateTime.Now).ToList();
			if (events.Count != 0)
			{
                foreach (var item in events)
                {
                    //var achorsEmail = item.EventAnchors.Split(',');

                    //  Construct the content of the mail

                    //var cont = "This is to remind you about" { item.EventName}
                    var Anchormessage = new Message(item.EventAnchorsId, "Test email", "This is to remind you about");
                    _emailSender.SendEmail(Anchormessage);
                    var Attendeemessage = new Message(item.EventAttendeesId, "Test email", "This is the mail from church scheduler app.");
                    _emailSender.SendEmail(Attendeemessage);

                    //var message = new Message("platinny10@gmail.com,yemibutwhy@gmail.com,bolu2009@yahoo.com", "Test email", "This is the mail from church scheduler app.");
                    //_emailSender.SendEmail(message);
                    //return $"Mail sent from Hangfire";
                };
            }
        }

        public IEnumerable<Event> GetEvents()
		{
            var events = tasksDbContext.Events;
            return events;
        }

		public async Task<SaveEventResponse> SaveEvent(Event events)
		{
            if (events.Id == 0)
            {
                await tasksDbContext.Events.AddAsync(events);
            }
            else
            {
                var eventsRecord = await tasksDbContext.Events.FindAsync(events.Id);

                eventsRecord.EventName = events.EventName;
                eventsRecord.EventAttendeesId = events.EventAttendeesId;
                eventsRecord.EventAnchorsId = events.EventAnchorsId;
                eventsRecord.EventTime = events.EventTime;
                //eventsRecord.Message = events.Message;
                eventsRecord.Ts = DateTime.Now;
            }

            var saveResponse = await tasksDbContext.SaveChangesAsync();

            if (saveResponse >= 0)
            {
                return new SaveEventResponse
                {
                    Success = true,
                    Event = events
                };
            }
            return new SaveEventResponse
            {
                Success = false,
                Error = "Unable to save task",
                ErrorCode = "T05"
            };
        }

        public Event GetEvent(int eventId)
        {
            var events = tasksDbContext.Events.FirstOrDefault(x => x.Id == eventId);
            return events;
        }
    }
}
