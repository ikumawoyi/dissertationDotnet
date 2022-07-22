using EmailService;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using TasksApi.Entities;
using TasksApi.Interfaces;
using TasksApi.Requests;
using TasksApi.Responses;

namespace TasksApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OccasionsController : ControllerBase
	{
		private readonly IOccasionService occasionService;

		public OccasionsController(IOccasionService occasionService)
		{
			this.occasionService = occasionService;
		}
		[HttpGet]
		public IActionResult Get()
		{
			RecurringJob.AddOrUpdate(() => occasionService.SendEmail(), Cron.Minutely);
			var getOccasionsResponse = occasionService.GetOccasions();

			return Ok(getOccasionsResponse);
		}

		[HttpGet("{id}")]
		public  IActionResult GetOccasion(int id)
		{
			var getOccasionResponse = occasionService.GetOccasion(id);

			return Ok(getOccasionResponse);
		}


		[HttpPost]
		public async Task<IActionResult> Post(OccasionRequest occasionRequest)
		{
			var occasion = new Occasion { Ts = occasionRequest.Ts, Name = occasionRequest.Name, Anchors = occasionRequest.Anchors, Attendees = occasionRequest.Attendees, StartDate = occasionRequest.StartDate, Id = occasionRequest.Id };

			var saveOccasionResponse = await occasionService.SaveOccasion(occasion);

			if (!saveOccasionResponse.Success)
			{
				return UnprocessableEntity(saveOccasionResponse);
			}

			var occasionResponse = new OccasionResponse { Id = saveOccasionResponse.Occasion.Id, Name = saveOccasionResponse.Occasion.Name, Anchors = saveOccasionResponse.Occasion.Anchors, Ts = saveOccasionResponse.Occasion.Ts, Attendees = saveOccasionResponse.Occasion.Attendees, StartDate = saveOccasionResponse.Occasion.StartDate };

			return Ok(occasionResponse);
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (id == 0)
			{
				return BadRequest(new DeleteOccasionResponse { Success = false, ErrorCode = "D01", Error = "Invalid Task id" });
			}
			var deleteOccasionResponse = await occasionService.DeleteOccasion(id);
			if (!deleteOccasionResponse.Success)
			{
				return UnprocessableEntity(deleteOccasionResponse);
			}

			return Ok(deleteOccasionResponse.OccasionId);
		}
		

		[HttpPut]
		public async Task<IActionResult> Put(OccasionRequest occasionRequest)
		{
			var occasion = new Occasion { Ts = occasionRequest.Ts, Name = occasionRequest.Name, Attendees = occasionRequest.Attendees, Anchors = occasionRequest.Anchors, Id = occasionRequest.Id , StartDate = occasionRequest.StartDate };

			var saveOccasionResponse = await occasionService.SaveOccasion(occasion);

			if (!saveOccasionResponse.Success)
			{
				return UnprocessableEntity(saveOccasionResponse);
			}

			var occasionResponse = new OccasionResponse { Id = saveOccasionResponse.Occasion.Id, Name = saveOccasionResponse.Occasion.Name, Anchors = saveOccasionResponse.Occasion.Anchors, Ts = saveOccasionResponse.Occasion.Ts, Attendees = saveOccasionResponse.Occasion.Attendees , StartDate = saveOccasionResponse.Occasion.StartDate };

			return Ok(occasionResponse);
		}
	}
}
