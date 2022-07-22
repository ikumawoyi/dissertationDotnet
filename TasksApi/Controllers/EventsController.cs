using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TasksApi.Entities;
using TasksApi.Interfaces;
using TasksApi.Requests;
using TasksApi.Responses;

namespace TasksApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EventsController : ControllerBase
	{
		private readonly IEventService eventService;
		public EventsController(IEventService eventService)
		{
			this.eventService = eventService;
		}

		[HttpGet]
		public IActionResult Get()
		{
			RecurringJob.AddOrUpdate(() => eventService.SendEmail(), Cron.Minutely);
			var getTasksResponse = eventService.GetEvents();

			return Ok(getTasksResponse);
		}

		[HttpGet("{id}")]
		public IActionResult GetEvent(int id)
		{
			var getEventResponse = eventService.GetEvent(id);

			return Ok(getEventResponse);
		}
		[HttpPost]
		public async Task<IActionResult> Post(EventRequest eventRequest)
		{
			var events = new Event { EventName = eventRequest.EventName, Ts = eventRequest.Ts, EventAnchorsId = eventRequest.EventAnchorsId, EventAttendeesId = eventRequest.EventAttendeesId, EventTime = eventRequest.EventTime,  Id = eventRequest.Id };

			var saveEventResponse = await eventService.SaveEvent(events);

			if (!saveEventResponse.Success)
			{
				return UnprocessableEntity(saveEventResponse);
			}

			var eventResponse = new EventResponse { Id = saveEventResponse.Event.Id, EventName = saveEventResponse.Event.EventName, EventAnchorsId = saveEventResponse.Event.EventAnchorsId, Ts = saveEventResponse.Event.Ts, EventAttendeesId = saveEventResponse.Event.EventAttendeesId, EventTime = saveEventResponse.Event.EventTime };

			return Ok(eventResponse);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (id == 0)
			{
				return BadRequest(new DeleteEventResponse { Success = false, ErrorCode = "D01", Error = "Invalid Task id" });
			}
			var deleteEventResponse = await eventService.DeleteEvent(id);
			if (!deleteEventResponse.Success)
			{
				return UnprocessableEntity(deleteEventResponse);
			}

			return Ok(deleteEventResponse.EventId);
		}

		[HttpPut]
		public async Task<IActionResult> Put(EventRequest eventRequest)
		{
			var events = new Event { EventName = eventRequest.EventName, Ts = eventRequest.Ts, EventAnchorsId = eventRequest.EventAnchorsId,  EventAttendeesId = eventRequest.EventAttendeesId,  EventTime = eventRequest.EventTime, Id = eventRequest.Id };

			var saveEventResponse = await eventService.SaveEvent(events);

			if (!saveEventResponse.Success)
			{
				return UnprocessableEntity(saveEventResponse);
			}

			var eventResponse = new EventResponse { Id = saveEventResponse.Event.Id, EventName = saveEventResponse.Event.EventName, EventAnchorsId = saveEventResponse.Event.EventAnchorsId, Ts = saveEventResponse.Event.Ts,  EventAttendeesId = saveEventResponse.Event.EventAttendeesId,  EventTime = saveEventResponse.Event.EventTime };

			return Ok(eventResponse);
		}

	}
}
