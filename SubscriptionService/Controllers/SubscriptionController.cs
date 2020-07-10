using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using System.Threading.Tasks;

namespace SubscriptionService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly AppDbContext context;

        public SubscriptionController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await context.Client.AsNoTracking().ToListAsync());
        }
    }
}
