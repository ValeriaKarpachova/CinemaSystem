using Cinema.Domain.Common;
using Cinema.Domain.Entities;
using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Web.Services;

public class SessionService : ISessionService
{
    private readonly ISessionRepository _repo;

    public SessionService(ISessionRepository repo) => _repo = repo;

    public Task<List<Session>> SearchAsync(DateOnly? date, int? movieId, int? hallId) =>
        _repo.SearchAsync(date, movieId, hallId);

    public async Task<OperationResult> SaveAsync(Session session)
    {
        if (session.MovieId == 0) return OperationResult.Fail("Оберіть фільм.");
        if (session.HallId == 0) return OperationResult.Fail("Оберіть зал.");
        if (session.SessionDate < DateOnly.FromDateTime(DateTime.Today))
            return OperationResult.Fail("Дата сеансу не може бути в минулому.");

        int? excludeId = session.SessionId == 0 ? null : session.SessionId;

        // перевірка на рівні сервісу: швидке й зрозуміле повідомлення
        if (await _repo.ConflictExistsAsync(session.HallId, session.SessionDate, session.SessionTime, excludeId))
            return OperationResult.Fail("У цьому залі вже є сеанс на обрані дату й час.");

        try
        {
            if (session.SessionId == 0) await _repo.AddAsync(session);
            else await _repo.UpdateAsync(session);
        }
        catch (DbUpdateException)
        {
            // підстраховка: обмеження UNIQUE (hall_id, session_date, session_time) з боку БД,
            // якщо два адміністратори зберегли конфліктний сеанс одночасно
            return OperationResult.Fail("Конфлікт розкладу: сеанс у цьому залі на цей час щойно з'явився.");
        }

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
            return OperationResult.Fail("Неможливо видалити сеанс: на нього вже є квитки.");
        }
    }
}