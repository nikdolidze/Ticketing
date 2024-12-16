using Ticketing.Core.Domain.Basics;

namespace Ticketing.Core.Domain.Entities;

public class Price : BaseEntity<int>
{
    public decimal Amount { get; set; }
}