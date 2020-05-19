using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Baccarat.Controllers
{
    /// <summary>
    /// 游戏信息
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAllOrigin")]
    public class GameInfoController : Controller
    {

    }
}
