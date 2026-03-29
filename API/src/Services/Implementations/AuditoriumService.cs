using API.Domain.Common;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;

namespace API.Services.Implementations;

/// <summary>
/// Provides services for managing auditoriums, including operations such as retrieval, creation, updating,
/// and deletion. Serves as a service layer between the repositories and controllers to handle auditorium-related data.
/// </summary>
public class AuditoriumService : IAuditoriumService
{
    /// <summary>
    /// Repository interface instance used to perform data access operations
    /// for auditorium entities. It acts as a bridge to interact with the underlying
    /// data source, enabling CRUD functionality for auditorium-related data.
    /// </summary>
    private readonly IAuditoriumRepository _auditoriumRepository;

    /// <summary>
    /// Provides functionality for managing auditorium-related operations.
    /// Acts as a service layer between the controller and repository
    /// for handling auditorium data.
    /// </summary>
    public AuditoriumService(IAuditoriumRepository auditoriumRepository)
    {
        _auditoriumRepository = auditoriumRepository;
    }

    /// <summary>
    /// Retrieves information about a specific auditorium based on its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the auditorium to be retrieved.</param>
    /// <returns>An asynchronous task that returns a result wrapping the auditorium details if successful, or an error message if the retrieval fails.</returns>
    public async Task<ResultOf<Auditorium>> GetAuditoriumAsync(int id)
    {
        var result = await _auditoriumRepository.GetAuditoriumAsync(id);
        return result;
    }

    /// <summary>
    /// Asynchronously retrieves a collection of auditoriums from the data repository.
    /// </summary>
    /// <returns>
    /// A ResultOf object containing a collection of Auditorium entities upon successful operation,
    /// or detailed error information if the operation fails.
    /// </returns>
    public async Task<ResultOf<ICollection<Auditorium>>> GetAuditoriumsAsync()
    {
        return await _auditoriumRepository.GetAuditoriumsAsync();
    }

    /// <summary>
    /// Asynchronously adds a new auditorium to the system using the provided details.
    /// </summary>
    /// <param name="auditorium">
    /// An instance of <see cref="CreateAuditoriumRequest"/> containing the details of the auditorium to be added,
    /// including its name and row configuration.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result of type <see cref="Auditorium"/>
    /// that includes the newly created auditorium details upon successful completion.
    /// </returns>
    public async Task<Auditorium> AddAuditoriumAsync(CreateAuditoriumRequest auditorium)
    {
        return await _auditoriumRepository.AddAuditoriumAsync(auditorium);
    }

    /// <summary>
    /// Asynchronously updates an existing auditorium entity in the data source.
    /// </summary>
    /// <param name="auditorium">
    /// The auditorium entity to be updated, containing properties such as
    /// name, row configuration, or other details that need modification.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains
    /// the updated auditorium entity after persisting the changes in the data source.
    /// </returns>
    public async Task<Auditorium> UpdateAuditoriumAsync(Auditorium auditorium)
    {
        return await _auditoriumRepository.UpdateAuditoriumAsync(auditorium);
    }

    /// <summary>
    /// Deletes an auditorium with the specified ID asynchronously.
    /// This operation ensures the removal of the auditorium from the system
    /// by delegating the data deletion process to the repository layer.
    /// </summary>
    /// <param name="auditoriumId">The unique identifier of the auditorium to be deleted.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a <see cref="ResultOf{T}"/> object indicating whether the operation was successful and,
    /// if successful, includes the deleted auditorium entity.
    /// </returns>
    public async Task<ResultOf<Auditorium>> DeleteAuditoriumByIdAsync(int auditoriumId)
    {
        return await _auditoriumRepository.DeleteAuditoriumByIdAsync(auditoriumId);
    }
}