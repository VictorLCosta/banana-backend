using Application.Rooms;

namespace Server.Controllers;

public class RoomsController : BaseApiController
{
    [HttpGet]
    public async Task<Result<List<RoomDto>>> GetRooms()
    {
        var result = await Mediator.Send(new GetRoomsQuery());

        return result;
    }
}
