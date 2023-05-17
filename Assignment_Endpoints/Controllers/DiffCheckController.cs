using Assignment_Endpoints.Models;
using Assignment_Endpoints.Services;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_Endpoints.Controllers
{
    [ApiController]
    [Route("v1/diff")]
    public class DiffCheckController : ControllerBase
    {

        private readonly DiffService DiffService;
        public DiffCheckController(DiffService diffService)
        {
            // Dependency injection for DiffService
            DiffService = diffService;
        }

        static Dictionary<int, string> InputsLeft = new Dictionary<int, string>();
        static Dictionary<int, string> InputsRight = new Dictionary<int, string>();

        [HttpPost("{id}/left")]
        public IActionResult SetLeft(int id, [FromBody] InputModel model)
        {
            var base64EncodedJson = model.Input;
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedJson));
            InputsLeft[id] = json;

            return Ok();
        }

        [HttpPost("{id}/right")]
        public IActionResult SetRight(int id, [FromBody] InputModel model)
        {
            var base64EncodedJson = model.Input;
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedJson));
            InputsRight[id] = json;

            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult Diff(int id)
        {
            if (!InputsLeft.ContainsKey(id) || !InputsRight.ContainsKey(id))
            {
                return BadRequest("Not all inputs were provided for this id");
            }

            string left = InputsLeft[id];
            string right = InputsRight[id];

            // The actual diff finding logic is in DiffService class (separation of concerns)
            var diffResult = DiffService.GetDiff(left, right);

            return Ok(diffResult);
        }



    }
}
