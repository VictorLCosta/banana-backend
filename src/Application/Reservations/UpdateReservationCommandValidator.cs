namespace Application.Reservations;

public class UpdateReservationCommandValidator : AbstractValidator<UpdateReservationCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateReservationCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Reservation.Description).NotEmpty().WithMessage("Description is required.");
        RuleFor(x => x.Reservation.ReservationDate).NotEmpty().WithMessage("Reservation date is required.");
        RuleFor(x => x.Reservation.StartTime).NotEmpty().WithMessage("Start time is required.");
        RuleFor(x => x.Reservation.EndTime).NotEmpty().WithMessage("End time is required.");
        RuleFor(x => x.Reservation.RoomId).NotEmpty().WithMessage("Room ID is required.");

        RuleFor(x => x.Reservation.EndTime)
            .GreaterThan(x => x.Reservation.StartTime)
            .WithMessage("End time should be greater than start time.");

        RuleFor(x => x.Reservation.CoffeePeopleCount)
            .GreaterThan(0)
            .NotNull()
            .When(x => x.Reservation.CoffeeIncluded)
            .WithMessage("Coffee people count must be greater than 0 when coffee is included.");

        RuleFor(x => x)
            .MustAsync(NoDateConflict)
            .WithMessage("There is already a reservation that conflicts with the selected time range.");
    }

    private async Task<bool> NoDateConflict(UpdateReservationCommand command, CancellationToken cancellationToken)
    {
        var reservation = command.Reservation;

        var existsConflict = await _context.Reservations.AnyAsync(
            x => x.RoomId == reservation.RoomId &&
            x.Id != command.Id &&
            x.ReservationDate == reservation.ReservationDate &&
            reservation.StartTime < x.EndTime &&
            reservation.EndTime > x.StartTime,
        cancellationToken);

        return !existsConflict;
    }
}
