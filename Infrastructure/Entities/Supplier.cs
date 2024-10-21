using System.Text.Json.Serialization;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string ContactPerson { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;

    [JsonIgnore]
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}