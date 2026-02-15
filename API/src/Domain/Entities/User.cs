namespace WebApi_PocV1.Domain.Entities
{
    public class User
    {
        public int Id { get; private set; }     // EF kan private set
        public string Name { get; private set; } = null!;

        // EF Core constructor
        private User() { }

        public User(string name)
        {
            Name = name;
        }
    }
}