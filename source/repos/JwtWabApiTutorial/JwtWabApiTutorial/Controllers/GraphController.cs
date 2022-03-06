using JwtWabApiTutorial.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtWabApiTutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraphController : ControllerBase
    {
        private readonly DataContext _context;
        public GraphController(DataContext context)
        {
            _context = context;
        }
        [HttpGet("GraphData")]
        public ActionResult<List<GraphValue>> GraphValue()
        {
            return _context.graphValues.ToList();
        }
        [HttpPost("SaveGraphValue")]
        public ActionResult<string> SaveGraphValue(GraphValue value)
        {
            _context.graphValues.Add(value);
            _context.SaveChanges();
            return Ok(new
            {
                message = "success"
            });
        }

    }
}
