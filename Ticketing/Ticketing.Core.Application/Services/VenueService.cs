using Ticketing.Core.Application.DTOs.Sections;
using Ticketing.Core.Application.DTOs.Venues;
using Ticketing.Core.Application.Interfaces;

namespace Ticketing.Core.Application.Services;

public class VenueService : IVenueService
{
    private readonly IUnitOfWork _unitOfWork;

    public VenueService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<VenueDto>> GetAll()
    {
        var venue = await _unitOfWork.VenueRepository.ReadAsync();
        return venue.Select(x => new VenueDto
        {
            Name = x.Name,
            Location = x.Location
        }).ToList();
    }

    public async Task<VenueDto?> GetById(int id)
    {
        var venue = await _unitOfWork.VenueRepository.ReadAsync(id);
        if (venue == null) return default;
        return new VenueDto
        {
            Name = venue.Name,
            Location = venue.Location
        };
    }

    public async Task<List<SectionDto>> GetSections(int venueId)
    {
        var venue = await _unitOfWork.VenueRepository.ReadAsync(venueId, x => x.Sections);
        var sections = venue!.Sections;
        return sections.Select(x => new SectionDto
        {
            Name = x.Name
        }).ToList();
    }
}

public interface IVenueService
{
    public Task<List<VenueDto>> GetAll();

    public Task<VenueDto?> GetById(int id);

    public Task<List<SectionDto>> GetSections(int venueId);
}