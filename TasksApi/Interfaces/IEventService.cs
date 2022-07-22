using TasksApi.Entities;
using TasksApi.Responses;

namespace TasksApi.Interfaces
{
	public interface IEventService
	{
		void SendEmail();
		IEnumerable<Event> GetEvents();
		Task<DeleteEventResponse> DeleteEvent(int eventId);

		Event GetEvent(int eventId);
		Task<SaveEventResponse> SaveEvent(Event events );

	}
}
