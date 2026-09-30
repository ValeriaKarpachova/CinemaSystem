using Cinema.Domain.Common;
using Cinema.Domain.Entities;
using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces;

namespace Cinema.Web.Services;

public class SeatService : ISeatService
{
    private const decimal MaxPrice = 10000m;

    private readonly ISeatRepository _seats;
    private readonly IHallRepository _halls;

    public SeatService(ISeatRepository seats, IHallRepository halls)
    {
        _seats = seats;
        _halls = halls;
    }

    public Task<List<Seat>> GetByHallAsync(int hallId) => _seats.GetByHallAsync(hallId);

    public async Task<OperationResult> SaveAsync(Seat seat)
    {
        var hall = await _halls.GetByIdAsync(seat.HallId);
        if (hall is null) return OperationResult.Fail("Зал не знайдено.");

        if (seat.RowNumber < 1 || seat.RowNumber > hall.RowsCount)
            return OperationResult.Fail($"Номер ряду має бути від 1 до {hall.RowsCount}.");
        if (seat.SeatNumber is < 1 or > 200)
            return OperationResult.Fail("Номер місця має бути від 1 до 200.");
        var priceError = ValidatePrice(seat.Price);
        if (priceError is not null) return OperationResult.Fail(priceError);

        int? excludeId = seat.SeatId == 0 ? null : seat.SeatId;
        if (await _seats.PositionExistsAsync(seat.HallId, seat.RowNumber, seat.SeatNumber, excludeId))
            return OperationResult.Fail("Місце з таким рядом і номером у цьому залі вже існує.");

        if (seat.SeatId == 0) await _seats.AddAsync(seat);
        else await _seats.UpdateAsync(seat);

        await _halls.RecalculateCapacityAsync(seat.HallId);
        return OperationResult.Ok();
    }

    public async Task<OperationResult> DeleteAsync(Seat seat)
    {
        if (await _seats.CountByHallAsync(seat.HallId) <= 1)
            return OperationResult.Fail("Не можна видалити останнє місце залу.");

        try
        {
            await _seats.DeleteAsync(seat.SeatId);
        }
        catch (RelatedRecordsExistException)
        {
            return OperationResult.Fail("Неможливо видалити місце: на нього є квитки.");
        }

        await _halls.RecalculateCapacityAsync(seat.HallId);
        return OperationResult.Ok();
    }

    public async Task<OperationResult> SetRowPriceAsync(int hallId, int row, decimal price)
    {
        var priceError = ValidatePrice(price);
        if (priceError is not null) return OperationResult.Fail(priceError);

        var changed = await _seats.SetRowPriceAsync(hallId, row, price);
        return changed == 0
            ? OperationResult.Fail("У цьому ряду немає місць.")
            : OperationResult.Ok();
    }

    private static string? ValidatePrice(decimal price) =>
        price < 0 || price > MaxPrice ? $"Ціна має бути від 0 до {MaxPrice:0} грн." : null;
}