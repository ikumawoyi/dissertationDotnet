namespace TasksApi.Responses
{
    public class GetTasksResponse : BaseResponse
    {
        public List<Task> Members { get; set; }
    }
}
