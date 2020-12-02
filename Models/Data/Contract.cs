namespace API.Models.Data
{
    public class Contract : BaseEntity
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
    }
}