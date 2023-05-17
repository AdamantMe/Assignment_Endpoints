using Assignment_Endpoints.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_Endpoints.Controllers
{
    [ApiController]
    [Route("v1/diff")]
    public class DiffCheckController : ControllerBase
    {
        //public IActionResult Index()
        //{

        //}

        private static Dictionary<int, string> DiffsLeft = new Dictionary<int, string>();
        private static Dictionary<int, string> DiffsRight = new Dictionary<int, string>();


        [HttpPost("{id}/left")]
        public IActionResult SetLeft(int id, [FromBody] InputModel model)
        {
            var base64EncodedJson = model.Input;
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedJson));
            DiffsLeft[id] = json;



            return Ok();
        }




    }
}
