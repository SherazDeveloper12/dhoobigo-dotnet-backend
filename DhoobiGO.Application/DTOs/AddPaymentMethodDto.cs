namespace DhoobiGO.Application.DTOs;

public class AddPaymentMethodDto
{
    public string Brand { get; set; } = string.Empty;
    public string Last4 { get; set; } = string.Empty;
    public string ExpiryDate { get; set; } = string.Empty;
}
