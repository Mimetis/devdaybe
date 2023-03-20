using Dotmim.Sync;
using Dotmim.Sync.Web.Server;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Speakers.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase {
        private WebServerAgent webServerAgent;

        // Injected thanks to Dependency Injection
        public SyncController(WebServerAgent webServerAgent) {
            this.webServerAgent = webServerAgent;
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
        public async Task Get() 
            => await this.HttpContext.WriteHelloAsync(webServerAgent);

    }
}
