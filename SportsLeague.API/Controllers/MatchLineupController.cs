using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/match/{matchId}/lineup")]
    public class MatchLineupController : ControllerBase
    {
        private readonly IMatchLineupService _service;
        private readonly IMapper _mapper;

        public MatchLineupController(
            IMatchLineupService service,
            IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // POST Agregar jugador a alineación
        [HttpPost]
        public async Task<ActionResult<MatchLineupDto>>
            Create(
                int matchId,
                CreateMatchLineupDto dto)
        {
            try
            {
                var lineup =
                    _mapper.Map<MatchLineup>(dto);

                lineup.MatchId = matchId;

                var created =
                    await _service.CreateAsync(lineup);

                // Recargar con Player y Team
                var lineups =
                    await _service.GetByMatchAsync(matchId);

                var createdLineup =
                    lineups.FirstOrDefault(x => x.Id == created.Id);

                var response =
                    _mapper.Map<MatchLineupDto>(createdLineup);

                return CreatedAtAction(
                    nameof(GetByMatch),
                    new { matchId = response.MatchId },
                    response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        // GET Obtener alineación completa
        [HttpGet]
        public async Task<ActionResult<
            IEnumerable<MatchLineupDto>>>
            GetByMatch(int matchId)
        {
            var lineups =
                await _service.GetByMatchAsync(matchId);

            var response =
                _mapper.Map<IEnumerable<MatchLineupDto>>
                (lineups);

            return Ok(response);
        }

        // GET Obtener alineación por equipo
        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<
            IEnumerable<MatchLineupDto>>>
            GetByMatchAndTeam(
                int matchId,
                int teamId)
        {
            var lineups =
                await _service
                    .GetByMatchAndTeamAsync(
                        matchId,
                        teamId);

            var response =
                _mapper.Map<IEnumerable<MatchLineupDto>>
                (lineups);

            return Ok(response);
        }

        // DELETE Eliminar jugador de alineación
        [HttpDelete("{id}")]
        public async Task<ActionResult>
            Delete(
                int matchId,
                int id)
        {
            try
            {
                await _service.DeleteAsync(id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
