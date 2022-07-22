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
	public class MembersController : ControllerBase
	{
		private readonly IMemberService memberService;

		public MembersController(IMemberService memberService)
		{
			this.memberService = memberService;
		}
		[HttpGet]
		public IActionResult Get()
		{
			RecurringJob.AddOrUpdate(() => memberService.SendEmail(), Cron.Minutely);
			var getTasksResponse = memberService.GetMembers();

			return Ok(getTasksResponse);
		}

		[HttpGet("{id}")]
		public  IActionResult GetMember(int id)
		{
			var getMemberResponse = memberService.GetMember(id);

			return Ok(getMemberResponse);
		}


		[HttpPost]
		public async Task<IActionResult> Post(MemberRequest memberRequest)
		{
			var member = new Member { Address = memberRequest.Address, Ts = memberRequest.Ts, Name = memberRequest.Name, Email = memberRequest.Email, PhoneNumber = memberRequest.PhoneNumber, DateOfBirth = memberRequest.DateOfBirth, Id = memberRequest.Id };

			var saveMemberResponse = await memberService.SaveMember(member);

			if (!saveMemberResponse.Success)
			{
				return UnprocessableEntity(saveMemberResponse);
			}

			var memberResponse = new MemberResponse { Id = saveMemberResponse.Member.Id, Name = saveMemberResponse.Member.Name, Email = saveMemberResponse.Member.Email, Ts = saveMemberResponse.Member.Ts, Address = saveMemberResponse.Member.Address, PhoneNumber = saveMemberResponse.Member.PhoneNumber, DateOfBirth = saveMemberResponse.Member.DateOfBirth };

			return Ok(memberResponse);
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (id == 0)
			{
				return BadRequest(new DeleteMemberResponse { Success = false, ErrorCode = "D01", Error = "Invalid Task id" });
			}
			var deleteMemberResponse = await memberService.DeleteMember(id);
			if (!deleteMemberResponse.Success)
			{
				return UnprocessableEntity(deleteMemberResponse);
			}

			return Ok(deleteMemberResponse.MemberId);
		}
		

		[HttpPut]
		public async Task<IActionResult> Put(MemberRequest memberRequest)
		{
			var member = new Member { Address = memberRequest.Address, Ts = memberRequest.Ts, Name = memberRequest.Name, Email = memberRequest.Email, PhoneNumber = memberRequest.PhoneNumber, Id = memberRequest.Id , DateOfBirth = memberRequest.DateOfBirth };

			var saveMemberResponse = await memberService.SaveMember(member);

			if (!saveMemberResponse.Success)
			{
				return UnprocessableEntity(saveMemberResponse);
			}

			var memberResponse = new MemberResponse { Id = saveMemberResponse.Member.Id, Name = saveMemberResponse.Member.Name, Email = saveMemberResponse.Member.Email, Ts = saveMemberResponse.Member.Ts, Address = saveMemberResponse.Member.Address, PhoneNumber = saveMemberResponse.Member.PhoneNumber , DateOfBirth = saveMemberResponse.Member.DateOfBirth };

			return Ok(memberResponse);
		}
	}
}
