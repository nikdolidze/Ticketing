using Ticketing.Core.Domain.Basics;

namespace Ticketing.Core.Domain.Entities;

public class Venue : BaseEntity<int>
{
    public string Name { get; set; }

    public string Location { get; set; }

    public List<Section> Sections { get; set; } = new();
}