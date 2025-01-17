using Microsoft.AspNetCore.Mvc;

namespace TVShowTracker.API.Controllers;

public class BaseController<T> : ControllerBase  where T : class
{
}
