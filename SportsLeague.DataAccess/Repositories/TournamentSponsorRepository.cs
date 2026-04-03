using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories
{

    public class TournamentSponsorRepository
         : GenericRepository<TournamentSponsor>, ITournamentSponsorRepository
    {
        public TournamentSponsorRepository(LeagueDbContext context) : base(context)
        {

        }

        // Obtener relación específica
        public async Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(
            int tournamentId, int sponsorId)
        {
            return await _dbSet
                .Where(ts => ts.TournamentId == tournamentId && ts.SponsorId == sponsorId)
                .FirstOrDefaultAsync();
        }

        //Obtener sponsors de un torneo
        public async Task<IEnumerable<TournamentSponsor>> GetByTournamentAsync(
            int tournamentId)
        {
            return await _dbSet
                .Where(ts => ts.TournamentId == tournamentId)
                .Include(ts => ts.Sponsor)
                .ToListAsync();
        }

        // Obtener torneos de un sponsor
        public async Task<IEnumerable<TournamentSponsor>> GetBySponsorAsync(
            int sponsorId)
        {
            return await _dbSet
                .Where(ts => ts.SponsorId == sponsorId)
                .Include(ts => ts.Tournament)
                .ToListAsync();
        }
    }
}

