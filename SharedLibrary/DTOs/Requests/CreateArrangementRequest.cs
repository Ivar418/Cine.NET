namespace SharedLibrary.DTOs.Requests;

public class CreateArrangementRequest
{
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public List<CreateArrangementItemRequest> Items { get; set; } = new();
}

public class CreateArrangementItemRequest
{
    public int Type { get; set; }
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
}