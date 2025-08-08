namespace Application.Rooms;

public class GetRoomsQuery : IQuery<List<RoomDto>>;

public class GetRoomsQueryHandler(IApplicationDbContext context) : IQueryHandler<GetRoomsQuery, List<RoomDto>>
{

    public async Task<Result<List<RoomDto>>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
        => Result.Success(
            await context
                .Rooms
                .AsNoTracking()
                .ProjectToType<RoomDto>()
                .ToListAsync(cancellationToken)
        );
}
