using DhoobiGO.Application.DTOs;

namespace DhoobiGO.Application.Interfaces;

public interface IPaymentService
{
    Task<bool> ProcessPaymentAsync(PaymentRequestDto dto);
    Task<object?> GetPaymentStatusAsync(int orderId);
    Task<decimal> GetWalletBalanceAsync(int userId);
    Task<IEnumerable<object>> GetSavedCardsAsync(int userId);
    Task<bool> AddPaymentMethodAsync(int userId, AddPaymentMethodDto dto);
    Task<bool> AdjustBalanceAsync(int userId, decimal amount, string description);
    Task<bool> CreateWithdrawalRequestAsync(int userId, decimal amount, string method);
    Task<object?> GetWalletAsync(int userId);
}
