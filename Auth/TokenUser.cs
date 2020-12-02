namespace API.Auth
{
    public class TokenUser
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string TenantCodes { get; set; }
        public bool IsAdmin { get; set; }

    }
}