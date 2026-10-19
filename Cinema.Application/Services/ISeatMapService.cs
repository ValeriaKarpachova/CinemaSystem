using Cinema.Application.Models;
namespace Cinema.Application.Services;

public interface ISeatMapService
{
    Task<SeatMap?> GetAsync(int sessionId);
}