using TasksApi.Entities;
using TasksApi.Responses;

namespace TasksApi.Interfaces
{
	public interface IOccasionService
	{
		void SendEmail();
		IEnumerable<Occasion> GetOccasions();

		Task<SaveOccasionResponse> SaveOccasion(Occasion occasion);
		Occasion GetOccasion(int occasionId);

		Task<DeleteOccasionResponse> DeleteOccasion(int occasionId);
	}
}
