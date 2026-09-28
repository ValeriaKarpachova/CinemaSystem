using Cinema.Domain.Common;
using Cinema.Domain.Entities;

namespace Cinema.Web.Services;

public interface IHallService
{
    Task<List<Hall>> GetAllAsync();
    Task<Hall?> GetByIdAsync(int id);
    Task<OperationResult> CreateAsync(Hall hall, int seatsPerRow);
    Task<OperationResult> UpdateAsync(Hall hall);
    Task<OperationResult> DeleteAsync(int id);
}