using Application.Reservations;

namespace Server.Controllers;

public class ReservationsController : BaseApiController
{
    [HttpGet("{id}")]
    public async Task<Result<ReservationDto>> GetReservationById(Guid id)
    {
        var result = await Mediator.Send(new GetReservationByIdQuery(id));

        return result;
    }

    [HttpGet]
    public async Task<Result<PagedList<ReservationDto>>> GetReservations(int pageNumber = 1, int pageSize = 10)
    {
        var result = await Mediator.Send(new GetReservationsQuery() { PageNumber = pageNumber, PageSize = pageSize });

        return result;
    }

    [HttpPost]
    public async Task<Result<ReservationDto>> CreateReservation([FromBody] CreateReservationDto reservation)
    {
        var result = await Mediator.Send(new CreateReservationCommand(reservation));

        return result;
    }

    [HttpPut]
    public async Task<Result<ReservationDto>> UpdateReservation(UpdateReservationDto reservation)
    {
        var result = await Mediator.Send(new UpdateReservationCommand(reservation));

        return result;
    }

    [HttpDelete("{id}")]
    public async Task<Result<ReservationDto>> DeleteReservation(Guid id)
    {
        var result = await Mediator.Send(new DeleteReservationCommand(id));

        return result;
    }

    [HttpPost("delete-many")]
    public async Task<Result<ReservationDto>> DeleteManyReservations([FromBody] DeleteManyReservationsCommand commnand)
    {
        var result = await Mediator.Send(commnand);

        return result;
    }
}
