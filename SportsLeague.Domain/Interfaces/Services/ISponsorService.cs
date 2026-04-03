using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ISponsorService
    {
        Task<IEnumerable<Sponsor>> GetAllAsync();
        Task<Sponsor?> GetByIdAsync(int id);
        Task<Sponsor> CreateAsync(Sponsor sponsor);
        Task UpdateAsync(int id, Sponsor sponsor);
        Task DeleteAsync(int id);

        // Relación N:M
        Task<TournamentSponsor> LinkSponsorToTournament(int sponsorId, TournamentSponsor entity);
        Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsor(int sponsorId);
        Task Unlink(int sponsorId, int tournamentId);
    }
}
