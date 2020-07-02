using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MsGraphAdvancedQueriesAndJsonBatchingSample.Models;
using Services;

namespace MsGraphAdvancedQueriesAndJsonBatchingSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly ApplicationService applicationService;

        public ApplicationController(ApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        [HttpGet]
        public Task<ApplicationResponseModel> Search([FromQuery] string searchText)
        {
            return applicationService.SearchAsync(searchText);
        }
    }
}