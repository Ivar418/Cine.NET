namespace Cine.NET_WA.Domain;

public sealed class User
{
    public int Id { get; }
    public string Name { get; }

    public User(int id, string name)
    {
        Id = id;
        Name = name;
    }
}