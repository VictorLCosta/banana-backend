using Domain.ValueObjects;

namespace Application.Rooms;

public class RoomDto : IDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public short Capacity { get; set; }
    
    public Address Location { get; set; } = null!;
}
