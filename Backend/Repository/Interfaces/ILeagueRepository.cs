using Backend.Models.DB;

namespace Backend.Repository.Interfaces;

public interface ILeagueRepository
{
    /// <summary>
    /// Выдает информацию о всех лигах
    /// </summary>
    /// <returns></returns>
    public Task<List<League>> GetLeagues();
}