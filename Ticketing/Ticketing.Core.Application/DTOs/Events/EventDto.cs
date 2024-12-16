namespace Ticketing.Core.Application.DTOs.Events;

public record EventDto
{
    public string Name { get; set; }

    public DateTime Date { get; set; }
}