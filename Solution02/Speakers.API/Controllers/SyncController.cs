using Dotmim.Sync;
using Dotmim.Sync.Web.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Speakers.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase {
        private WebServerAgent webServerAgent;
        private readonly IWebHostEnvironment env;

        // Injected thanks to Dependency Injection
        public SyncController(WebServerAgent webServerAgent, IWebHostEnvironment env) {
            this.webServerAgent = webServerAgent;
            this.env = env;
        }

        /// <summary>
        /// This POST handler is mandatory to handle all the sync process
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task Post()
            => webServerAgent.HandleRequestAsync(this.HttpContext);

        /// <summary>
        /// This GET handler is optional. It allows you to see the configuration hosted on the server
        /// </summary>
        [HttpGet]
        public async Task Get() {
            if (env.IsDevelopment()) {
                await this.HttpContext.WriteHelloAsync(webServerAgent);
            }
            else {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine("<!doctype html>");
                stringBuilder.AppendLine("<html>");
                stringBuilder.AppendLine("<title>Web Server properties</title>");
                stringBuilder.AppendLine("<body>");
                stringBuilder.AppendLine(" PRODUCTION. Write Whatever You Want Here ");
                stringBuilder.AppendLine("</body>");
                await this.HttpContext.Response.WriteAsync(stringBuilder.ToString());
            }
        }

    }
}
