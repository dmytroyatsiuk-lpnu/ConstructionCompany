namespace ConstructionCompany.Models.ModelsDTO
{
    public class TaskDTO
    {
        public int TaskId { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly? Deadline { get; set; }

        public string? Description { get; set; }
        public string? Status { get; set; }

        public int? BrigadeId { get; set; }
    }
}
