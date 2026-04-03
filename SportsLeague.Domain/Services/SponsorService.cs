using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _repo;
        private readonly ITournamentSponsorRepository _tsRepo;
        private readonly IGenericRepository<Tournament> _tournamentRepo;

        public SponsorService(
            ISponsorRepository repo,
            ITournamentSponsorRepository tsRepo,
            IGenericRepository<Tournament> tournamentRepo)
        {
            _repo = repo;
            _tsRepo = tsRepo;
            _tournamentRepo = tournamentRepo;
        }

        //GET ALL
        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        //GET BY ID
        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        //CREATE
        public async Task<Sponsor> CreateAsync(Sponsor sponsor)
        {
            //Nombre duplicado
            if (await _repo.ExistsByNameAsync(sponsor.Name))
                throw new InvalidOperationException("El nombre ya existe");

            //Email inválido
            if (string.IsNullOrWhiteSpace(sponsor.ContactEmail) || !sponsor.ContactEmail.Contains("@"))
                throw new InvalidOperationException("Email inválido");

            return await _repo.CreateAsync(sponsor);
        }

        //UPDATE
        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            var existing = await _repo.GetByIdAsync(id);

            //No existe
            if (existing == null)
                throw new KeyNotFoundException("Sponsor no encontrado");

            //Nombre duplicado (excepto el mismo)
            if (await _repo.ExistsByNameAsync(sponsor.Name) && existing.Name != sponsor.Name)
                throw new InvalidOperationException("Nombre duplicado");

            //Email inválido
            if (string.IsNullOrWhiteSpace(sponsor.ContactEmail) || !sponsor.ContactEmail.Contains("@"))
                throw new InvalidOperationException("Email inválido");

            existing.Name = sponsor.Name;
            existing.ContactEmail = sponsor.ContactEmail;
            existing.Phone = sponsor.Phone;
            existing.WebsiteUrl = sponsor.WebsiteUrl;
            existing.Category = sponsor.Category;
            existing.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(existing);
        }

        //DELETE
        public async Task DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);

            if (existing == null)
                throw new KeyNotFoundException("Sponsor no encontrado");

            await _repo.DeleteAsync(id);
        }

        //N:M LINK
        public async Task<TournamentSponsor> LinkSponsorToTournament(int sponsorId, TournamentSponsor entity)
        {
            //Sponsor no existe
            var sponsor = await _repo.GetByIdAsync(sponsorId);
            if (sponsor == null)
                throw new KeyNotFoundException("Sponsor no existe");

            //Tournament no existe
            var tournament = await _tournamentRepo.GetByIdAsync(entity.TournamentId);
            if (tournament == null)
                throw new KeyNotFoundException("Tournament no existe");

            //Duplicado
            var existingRelation = await _tsRepo
                .GetByTournamentAndSponsorAsync(entity.TournamentId, sponsorId);

            if (existingRelation != null)
                throw new InvalidOperationException("Ya está vinculado");

            //ContractAmount inválido
            if (entity.ContractAmount <= 0)
                throw new InvalidOperationException("Monto inválido");

            entity.SponsorId = sponsorId;
            entity.JoinedAt = DateTime.UtcNow;

            return await _tsRepo.CreateAsync(entity);
        }

        //GET TORNEOS POR SPONSOR
        public async Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsor(int sponsorId)
        {
            var sponsor = await _repo.GetByIdAsync(sponsorId);

            if (sponsor == null)
                throw new KeyNotFoundException("Sponsor no existe");

            return await _tsRepo.GetBySponsorAsync(sponsorId);
        }

        //UNLINK
        public async Task Unlink(int sponsorId, int tournamentId)
        {
            var relation = await _tsRepo
                .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);

            if (relation == null)
                throw new KeyNotFoundException("Relación no existe");

            await _tsRepo.DeleteAsync(relation.Id);
        }
    }
}