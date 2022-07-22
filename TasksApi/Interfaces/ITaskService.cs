using TasksApi.Responses;

namespace TasksApi.Interfaces
{
    public interface ITaskService
    {
        Task<GetTasksResponse> GetTasks(int userId);
        Task<GetTasksResponse> GetAllTasks();

        Task<SaveTaskResponse> SaveTask(Task task);

        Task<DeleteTaskResponse> DeleteTask(int taskId, int userId);
    }
}
