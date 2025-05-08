namespace ConstructionCompany.Models.ModelsDTO
{
    public class UserDTO
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public int? EmployeeId { get; set; } = null;
    }
}
