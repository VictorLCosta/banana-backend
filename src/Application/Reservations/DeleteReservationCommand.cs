namespace Application.Reservations;

public record DeleteReservationCommand(Guid Id) : ICommand;

public class DeleteReservationCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteReservationCommand>
{
    public async Task<Result> Handle(DeleteReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await context.Reservations
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (reservation is null) return Result.NotFound("Reservation not found");

        context.Reservations.Remove(reservation);

        return Result.NoContent();
    }
}
