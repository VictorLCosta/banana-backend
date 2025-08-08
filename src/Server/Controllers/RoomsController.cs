using Application.Rooms;

using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers;

[Authorize]
public class RoomsController : BaseApiController
{
    [HttpGet]
    public async Task<Result<List<RoomDto>>> GetRooms()
    {
        var result = await Mediator.Send(new GetRoomsQuery());

        return result;
    }
}
