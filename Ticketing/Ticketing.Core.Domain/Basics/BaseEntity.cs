namespace Ticketing.Core.Domain.Basics;

public abstract class BaseEntity<Tid>
{
    public Tid Id { get; set; }
}