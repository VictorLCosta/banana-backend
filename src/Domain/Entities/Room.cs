namespace Domain.Entities;

public class Room : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public short Capacity { get; set; }

    public Address Location { get; set; } = null!;

    public ICollection<Reservation> Reservations { get; set; } = [];
}
