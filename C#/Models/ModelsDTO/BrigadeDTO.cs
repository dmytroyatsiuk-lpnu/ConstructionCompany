namespace ConstructionCompany.Models.ModelsDTO
{
    public class BrigadeDTO
    {
        public int BrigadeId { get; set; }
        public DateOnly? CreatedAt { get; set; }

        public bool? IsActive { get; set; }

        public string? Specialization { get; set; }

        public string? Name { get; set; }
    }
}
