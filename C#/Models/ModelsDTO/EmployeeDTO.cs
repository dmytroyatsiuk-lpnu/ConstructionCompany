namespace ConstructionCompany.Models.ModelsDTO
{
    public class EmployeeDTO
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = null!;

        public DateOnly? HireDate { get; set; }

        public decimal? Salary { get; set; }

        public string? Position { get; set; }

        public int? BrigadeId { get; set; }
    }
}
