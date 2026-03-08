using SharedLibrary.Domain.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;


namespace API.Services.Implementations;

public class TicketService: ITicketService
{
        private readonly ITicketRepository _repository;

        public TicketService(ITicketRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<Ticket>> GetAllTicketsAsync()
            => await _repository.GetAllAsync();

        public async Task<Ticket?> GetTicketByIdAsync(int id)
            => await _repository.GetByIdAsync(id);


        public async Task<IReadOnlyList<Ticket>> GetMovieTicketsAsync(int movieId)
            => await _repository.GetTicketsByMovieIdAsync(movieId);

        public async Task<Ticket> CreateTicketAsync(Ticket ticket)
        {
            await _repository.AddAsync(ticket);
            return ticket;
        }

        public async Task UpdateTicketAsync(Ticket ticket)
            => await _repository.UpdateAsync(ticket);

        public async Task DeleteTicketAsync(int id)
            => await _repository.DeleteAsync(id);
}