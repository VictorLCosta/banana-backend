
using Domain.Entities;

namespace Application.Reservations;

public class GetReservationsQuery : PaginationFilter, IQuery<PagedList<ReservationDto>>;

public class GetReservationsQueryHandler(IApplicationDbContext context) : IQueryHandler<GetReservationsQuery, PagedList<ReservationDto>>
{
    public async Task<Result<PagedList<ReservationDto>>> Handle(GetReservationsQuery request, CancellationToken cancellationToken)
    {
        var reservations = await context
            .Reservations
            .AsNoTracking()
            .PaginatedListAsync<Reservation, ReservationDto>(null, request.PageNumber, request.PageSize, cancellationToken);

        return reservations;
    }
}
