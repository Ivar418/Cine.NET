namespace SharedLibrary.DTOs.Responses;

public class ArrangementResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }

    public List<ArrangementItemResponse> Items { get; set; } = new();
}

public class ArrangementItemResponse
{
    public string Type { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int Quantity { get; set; }
}