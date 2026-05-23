using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class MatchLineupService : IMatchLineupService
    {
        private readonly IMatchLineupRepository _lineupRepo;
        private readonly IGenericRepository<Match> _matchRepo;
        private readonly IGenericRepository<Player> _playerRepo;

        public MatchLineupService(
            IMatchLineupRepository lineupRepo,
            IGenericRepository<Match> matchRepo,
            IGenericRepository<Player> playerRepo)
        {
            _lineupRepo = lineupRepo;
            _matchRepo = matchRepo;
            _playerRepo = playerRepo;
        }

        // GET Listar alineaciones por partido
        public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId)
        {
            return await _lineupRepo.GetByMatchAsync(matchId);
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
        {
            return await _lineupRepo
                .GetByMatchAndTeamAsync(matchId, teamId);
        }

        // CREATE Registrar alineación
        public async Task<MatchLineup> CreateAsync(MatchLineup lineup)
        {
            // V1 El partido debe existir
            var match = await _matchRepo.GetByIdAsync(lineup.MatchId);

            if (match == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el partido con ID {lineup.MatchId}");
            }

            // V2 El jugador debe existir
            var player = await _playerRepo.GetByIdAsync(lineup.PlayerId);

            if (player == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el jugador con ID {lineup.PlayerId}");
            }

            // V3 El jugador debe pertenecer al HomeTeam o AwayTeam
            if (player.TeamId != match.HomeTeamId &&
                player.TeamId != match.AwayTeamId)
            {
                throw new InvalidOperationException(
                    "El jugador no pertenece a ninguno de los equipos del partido");
            }

            // V4 Evitar duplicados
            if (await _lineupRepo.ExistsAsync(
                lineup.MatchId,
                lineup.PlayerId))
            {
                throw new InvalidOperationException(
                    "El jugador ya está registrado en la alineación de este partido");
            }

            // V5 Máximo 11 titulares por equipo
            if (lineup.IsStarter)
            {
                var lineups =
                    await _lineupRepo.GetByMatchAsync(lineup.MatchId);

                var startersCount =
                    lineups.Count(x =>
                        x.IsStarter &&
                        x.Player.TeamId == player.TeamId);

                if (startersCount >= 11)
                {
                    throw new InvalidOperationException(
                        "El equipo ya tiene 11 titulares registrados en este partido");
                }
            }

            // V6 Match debe estar Scheduled
            if (match.Status != MatchStatus.Scheduled)
            {
                throw new InvalidOperationException(
                    "Solo se pueden registrar alineaciones en partidos Scheduled");
            }

            // Guardar
            return await _lineupRepo.CreateAsync(lineup);
        }

        // DELETE Eliminar alineación
        public async Task DeleteAsync(int id)
        {
            var lineup = await _lineupRepo.GetByIdAsync(id);

            if (lineup == null)
            {
                throw new KeyNotFoundException(
                    "Convocatoria no encontrada");
            }

            await _lineupRepo.DeleteAsync(id);
        }
    }
}
