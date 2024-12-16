using Ticketing.Core.Domain.Basics;

namespace Ticketing.Core.Domain.Entities;

public class Event : BaseEntity<int>
{
    public string Name { get; set; }

    public DateTime Date { get; set; }

    public Venue Venue { get; set; }
}