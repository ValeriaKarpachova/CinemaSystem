using Cinema.Domain.Common;
using Cinema.Domain.Entities;

namespace Cinema.Web.Services;

public interface ISeatService
{
    Task<List<Seat>> GetByHallAsync(int hallId);
    Task<OperationResult> SaveAsync(Seat seat);
    Task<OperationResult> DeleteAsync(Seat seat);
    Task<OperationResult> SetRowPriceAsync(int hallId, int row, decimal price);
}