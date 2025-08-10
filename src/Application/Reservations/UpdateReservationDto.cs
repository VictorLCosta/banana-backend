namespace Application.Reservations;

public class UpdateReservationDto : IDto
{
    public string? Description { get; set; }
    public string ResponsibleName { get; set; } = string.Empty;
    public DateOnly ReservationDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool CoffeeIncluded { get; set; }
    public int? CoffeePeopleCount { get; set; }
    public Guid RoomId { get; set; }
}
