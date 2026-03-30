using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ITournamentTeamRepository : IGenericRepository<TournamentTeam>
    {
        Task<TournamentTeam?> GetByTournamentAndTeamAsync(int tournamentId, int teamId); // Metodo para obtener una relacion especifica entre un torneo y un equipo
        Task<IEnumerable<TournamentTeam>> GetByTournamentAsync(int tournamentId); // Metodo para obtener todas las relaciones de equipos en un torneo especifico
        
    }

}
