namespace EntityFrameworkCore.Triggered.IntegrationTests.EntityBags
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}