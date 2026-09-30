using Cinema.Domain.Common;
using Cinema.Domain.Entities;
using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces;

namespace Cinema.Web.Services;

public class HallService : IHallService
{
    private const int MaxRows = 30;
    private const int MaxSeatsPerRow = 50;

    private readonly IHallRepository _repo;

    public HallService(IHallRepository repo) => _repo = repo;

    public Task<List<Hall>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Hall?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

    public async Task<OperationResult> CreateAsync(Hall hall, int seatsPerRow)
    {
        var error = await ValidateNameAndScreenAsync(hall);
        if (error is not null) return OperationResult.Fail(error);

        if (hall.RowsCount is < 1 or > MaxRows)
            return OperationResult.Fail($"Кількість рядів має бути від 1 до {MaxRows}.");
        if (seatsPerRow is < 1 or > MaxSeatsPerRow)
            return OperationResult.Fail($"Кількість місць у ряду має бути від 1 до {MaxSeatsPerRow}.");

        hall.Capacity = hall.RowsCount * seatsPerRow;

        var seats = new List<Seat>();
        for (int r = 1; r <= hall.RowsCount; r++)
            for (int s = 1; s <= seatsPerRow; s++)
                seats.Add(new Seat { RowNumber = r, SeatNumber = s, Price = PriceFor(r, hall.RowsCount) });

        await _repo.CreateWithSeatsAsync(hall, seats);
        return OperationResult.Ok();
    }

    public async Task<OperationResult> UpdateAsync(Hall hall)
    {
        var error = await ValidateNameAndScreenAsync(hall);
        if (error is not null) return OperationResult.Fail(error);

        var existing = await _repo.GetByIdAsync(hall.HallId);
        if (existing is null) return OperationResult.Fail("Зал не знайдено.");

        existing.HallName = hall.HallName;
        existing.ScreenType = hall.ScreenType;
        await _repo.UpdateAsync(existing);
        return OperationResult.Ok();
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        try
        {
            await _repo.DeleteAsync(id);
            return OperationResult.Ok();
        }
        catch (RelatedRecordsExistException)
        {
            return OperationResult.Fail("Неможливо видалити зал: для нього існують сеанси або квитки на його місця.");
        }
    }

    private static decimal PriceFor(int row, int rows) =>
        row <= 3 ? 150m : row <= rows - 3 ? 120m : 90m;

    private async Task<string?> ValidateNameAndScreenAsync(Hall hall)
    {
        hall.HallName = (hall.HallName ?? "").Trim();
        hall.ScreenType = string.IsNullOrWhiteSpace(hall.ScreenType) ? null : hall.ScreenType.Trim();

        if (hall.HallName.Length == 0) return "Назва залу не може бути порожньою.";
        if (hall.HallName.Length > 100) return "Назва залу не може перевищувати 100 символів.";
        if (hall.ScreenType?.Length > 50) return "Тип екрана не може перевищувати 50 символів.";

        int? excludeId = hall.HallId == 0 ? null : hall.HallId;
        if (await _repo.NameExistsAsync(hall.HallName, excludeId))
            return "Зал з такою назвою вже існує.";
        return null;
    }
}