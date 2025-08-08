namespace Application.Reservations;

public record DeleteManyReservationsCommand(Guid[] Ids) : ICommand;

public class DeleteManyReservationsCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteManyReservationsCommand>
{
    public async Task<Result> Handle(DeleteManyReservationsCommand request, CancellationToken cancellationToken)
    {
        var reservations = await context
            .Reservations
            .Where(x => request.Ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (reservations is null || reservations.Count == 0) return Result.NotFound("No reservations found for the given IDs.");

        context.Reservations.RemoveRange(reservations);

        return Result.NoContent();
    }
}
