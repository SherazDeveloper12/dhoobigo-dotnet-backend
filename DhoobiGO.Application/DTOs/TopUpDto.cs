namespace DhoobiGO.Application.DTOs;

public class TopUpDto
{
    public decimal Amount { get; set; }
    public int? UserId { get; set; }
    public string? Description { get; set; }
}

public class WithdrawalRequestDto
{
    public decimal Amount { get; set; }
    public string Method { get; set; } = "Bank";
}
