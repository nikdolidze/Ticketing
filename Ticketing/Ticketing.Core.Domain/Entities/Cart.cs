using Ticketing.Core.Domain.Basics;
using Ticketing.Core.Domain.Enums;

namespace Ticketing.Core.Domain.Entities;

public class Cart : BaseEntity<Guid>
{
    public List<CartItem> Items { get; set; } = new();

    public CartState State { get; set; }
}