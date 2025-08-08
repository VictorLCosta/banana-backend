namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public abstract class BaseApiController : ControllerBase
{
    private ISender _mediator = null!;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
