using Domain.Entities;

namespace Application.Reservations;

public record UpdateReservationCommand(UpdateReservationDto Reservation) : ICommand<ReservationDto>;

public class UpdateReservationCommandHandler(IApplicationDbContext context) : ICommandHandler<UpdateReservationCommand, ReservationDto>
{
    public async Task<Result<ReservationDto>> Handle(UpdateReservationCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateReservationCommandValidator(context);
        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            return Result.Invalid(result.AsErrors());
        }

        var reservation = await context
            .Reservations
            .FirstOrDefaultAsync(x => x.Id == request.Reservation.Id, cancellationToken);

        if (reservation == null)
                return Result.NotFound($"Reservation with ID: {request.Reservation.Id} not found.");

        var config = new TypeAdapterConfig()
            .NewConfig<UpdateReservationDto, Reservation>()
            .Ignore(x => x.CreatedAt)
            .Ignore(x => x.CreatedBy)
            .Ignore(x => x.Id)
            .Config;

        request.Reservation.Adapt(reservation, config);

        reservation.LastModified = DateTime.UtcNow;

        context.Reservations.Update(reservation);

        return Result<ReservationDto>.Success(reservation.Adapt<ReservationDto>());
    }
}
