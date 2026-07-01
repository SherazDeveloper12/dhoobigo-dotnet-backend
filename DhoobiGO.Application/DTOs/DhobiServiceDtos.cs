namespace DhoobiGO.Application.DTOs;

public class DhobiServiceDto
{
    public int Id { get; set; }
    public int ServiceTypeId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceIcon { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Unit { get; set; } = "pc";
    public bool IsActive { get; set; }
}

public class DhobiServiceCreateDto
{
    public int ServiceTypeId { get; set; }
    public decimal Price { get; set; }
    public string Unit { get; set; } = "pc";
}

public class DhobiServiceUpdateDto
{
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}
