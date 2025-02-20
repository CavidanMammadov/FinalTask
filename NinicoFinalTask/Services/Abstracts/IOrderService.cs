namespace NinicoFinalTask.Services.Abstracts
{
    public interface IOrderService
    {
        Task CreateOrderAsync(string userId, decimal amount);
        Task ConfirmOrderAsync(string sessionId);
    }
}
