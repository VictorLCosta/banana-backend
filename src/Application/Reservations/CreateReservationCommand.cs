using Domain.Entities;

namespace Application.Reservations;

public record CreateReservationCommand(CreateReservationDto Reservation) : ICommand<ReservationDto>;

public class CreateReservationCommandHandler(IApplicationDbContext context) : ICommandHandler<CreateReservationCommand, ReservationDto>
{
    public async Task<Result<ReservationDto>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateReservationCommandValidator(context);
        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            return Result.Invalid(result.AsErrors());
        }

        var reservation = request.Reservation.Adapt<Reservation>();

        await context.Reservations.AddAsync(reservation, cancellationToken);

        return Result<ReservationDto>.Created(reservation.Adapt<ReservationDto>());
    }
}
