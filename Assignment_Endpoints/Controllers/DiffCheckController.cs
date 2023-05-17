using Assignment_Endpoints.Models;
using Assignment_Endpoints.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace Assignment_Endpoints.Controllers
{
    [ApiController]
    // An attribute that sets the route template for all actions in this controller.
    [Route("v1/diff")]
    public class DiffCheckController : ControllerBase
    {

        private readonly DiffService _diffService;
        private readonly EncodingService _encodingService;

        public DiffCheckController(DiffService diffService, EncodingService encodingService)
        {
            // Constructor injection
            _diffService = diffService;
            _encodingService = encodingService;
        }

        // The dictionaries that store the left and right inputs for each ID.
        // They are static, so they are shared across all instances of this controller.
        // ConcurrentDictioary is a thread safe solution for multiple simultaneous requests. 

        // Note: In a real-world environment, these would more likely be stored in some kind of a long-term datastore (like a DB)
        // This in-memory implementation is not ideal but is suitable for a given assignment.
        static ConcurrentDictionary<int, string> InputsLeft = new ConcurrentDictionary<int, string>();
        static ConcurrentDictionary<int, string> InputsRight = new ConcurrentDictionary<int, string>();

        [HttpPost("{id}/left")]
        public IActionResult SetLeft(int id, [FromBody] InputModel model)
        {
            try
            {
                var json = _encodingService.DecodeBase64(model.Input);
                InputsLeft[id] = json;
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/right")]
        public IActionResult SetRight(int id, [FromBody] InputModel model)
        {
            try
            {
                var json = _encodingService.DecodeBase64(model.Input);
                InputsRight[id] = json;
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiffAsync(int id)
        {
            if (!InputsLeft.ContainsKey(id) || !InputsRight.ContainsKey(id))
            {
                return BadRequest("Not all inputs were provided for this id");
            }

            string left = InputsLeft[id];
            string right = InputsRight[id];

            var diffResult = await _diffService.GetDiffAsync(left, right);

            return Ok(diffResult);
        }


    }
}
