using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface IMatchLineupService
    {
        Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId);
        Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId);
        Task<MatchLineup> CreateAsync(MatchLineup lineup);
        Task DeleteAsync(int id);
    }

}
