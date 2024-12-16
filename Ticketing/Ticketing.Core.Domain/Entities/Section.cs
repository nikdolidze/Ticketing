using Ticketing.Core.Domain.Basics;

namespace Ticketing.Core.Domain.Entities;

public class Section : BaseEntity<int>
{
    public string Name { get; set; }

    public Venue Venue { get; set; }

    public List<Row> Rows { get; set; } = new();
}