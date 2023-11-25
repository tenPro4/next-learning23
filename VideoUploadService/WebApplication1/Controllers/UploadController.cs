using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController:ControllerBase
    {
        private readonly AppDbContext _context;

        public UploadController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetUpload(Guid id)
        {
            var upload = _context.Uploads.FirstOrDefault(x => x.Id == id);

            if(upload == null)
            {
                return BadRequest();
            }

            return Ok(upload);
        }
    }
}
