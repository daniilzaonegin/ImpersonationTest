using System.Runtime.Versioning;
using System.Security.Principal;
using FirstApp.Consts;
using Microsoft.AspNetCore.Mvc;

namespace FirstApp.Controllers
{
    [SupportedOSPlatform("windows")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserRightsDelegatingController : ControllerBase
    {
        private readonly ILogger<UserRightsDelegatingController> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public UserRightsDelegatingController(
            ILogger<UserRightsDelegatingController> logger,
            IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }
        [HttpGet]
        public async Task<IActionResult> CallUnderUserCredentials()
        {
            if (User.Identity is not WindowsIdentity windowsIdentity)
            {
                ModelState.AddModelError("User", "User must be authenticated with windows auth");
                return BadRequest(ModelState);
            }

            var result = await WindowsIdentity.RunImpersonatedAsync(windowsIdentity.AccessToken,
                async () =>
                {
                    HttpClient? client =
                        _clientFactory.CreateClient(ServiceConsts.ImpersonateClientName); 
                    var result = await client.GetAsync("http://localhost:5246/api/User");

                    return await result.Content.ReadAsStringAsync();
                });

            return Ok(result);
        }
    }
}
