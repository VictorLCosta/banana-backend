namespace Application.Reservations;

public record GetReservationByIdQuery(Guid Id) : IQuery<ReservationDto>;

public class GetReservationByIdQueryHandler(IApplicationDbContext context) : IQueryHandler<GetReservationByIdQuery, ReservationDto>
{
    public async Task<Result<ReservationDto>> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var reservation = await context
            .Reservations
            .ProjectToType<ReservationDto>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        return reservation == null ?
            Result.NotFound("Reservation not found.") :
            Result.Success(reservation);
    }
}
