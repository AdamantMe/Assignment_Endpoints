using Assignment_Endpoints.Controllers;
using Assignment_Endpoints.Models;
using Assignment_Endpoints.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Assignment_Endpoints.Tests
{
    public class DiffCheckControllerTests
    {

        private readonly DiffCheckController _controller;

        public DiffCheckControllerTests()
        {
            _controller = new DiffCheckController(new DiffService(), new EncodingService());
        }

        [Fact]
        public void SetLeft_GivenValidInput_GivesOkResult()
        {
            var input = Convert.ToBase64String(Encoding.UTF8.GetBytes("testing left"));
            var model = new InputModel { Input = input };
            var result = _controller.SetLeft(1, model);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void SetRight_GivenValidInput_GivesOkResult()
        {
            var input = Convert.ToBase64String(Encoding.UTF8.GetBytes("testing right"));
            var model = new InputModel { Input = input };
            var result = _controller.SetRight(1, model);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetDiff_GivenSameInputs_GivesIdenticalTrue()
        {
            var input = Convert.ToBase64String(Encoding.UTF8.GetBytes("input"));
            var model = new InputModel { Input = input };
            _controller.SetLeft(1, model);
            _controller.SetRight(1, model);

            // A cast to OkObjectResult, since it's an expected type when the test passes
            var result = await _controller.GetDiffAsync(1) as OkObjectResult;

            // Assert that nothing went wrong
            Assert.NotNull(result);
            var value = result.Value as Dictionary<string, object>;
            Assert.NotNull(value);
            // Verify that the "Identical" field in the result is set to false
            Assert.True((bool)value["Identical"]);
        }

        [Fact]
        public async Task GetDiff_GivenDifferentSizeInputs_GivesEqualSizeFalse()
        {
            var inputLeft = Convert.ToBase64String(Encoding.UTF8.GetBytes("left"));
            var inputRight = Convert.ToBase64String(Encoding.UTF8.GetBytes("right"));

            var modelLeft = new InputModel { Input = inputLeft };
            var modelRight = new InputModel { Input = inputRight };

            _controller.SetLeft(1, modelLeft);
            _controller.SetRight(1, modelRight);

            var result = await _controller.GetDiffAsync(1) as OkObjectResult;

            // Assert that nothing went wrong
            Assert.NotNull(result);
            var value = result.Value as Dictionary<string, object>;
            Assert.NotNull(value);
            // Verify that the "EqualSize" field in the result is set to false
            Assert.False((bool)value["EqualSize"]);
        }

        [Fact]
        public async Task GetDiff_GivenSameSizeDifferentInputs_GivesCorrectDiffs()
        {
            var inputLeft = Convert.ToBase64String(Encoding.UTF8.GetBytes("abcXd"));
            var inputRight = Convert.ToBase64String(Encoding.UTF8.GetBytes("abcYd"));

            _controller.SetLeft(1, new InputModel { Input = inputLeft });
            _controller.SetRight(1, new InputModel { Input = inputRight });

            var result = await _controller.GetDiffAsync(1) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

            var value = result.Value as Dictionary<string, object>;
            Assert.NotNull(value);

            Assert.True((bool)value["EqualSize"]);

            var diffs = value["Differences"] as List<Dictionary<string, object>>;
            Assert.NotNull(diffs);

            // We know there's only one difference
            Assert.Single(diffs);

            // The difference starts at the 3rd character (0-indexed)
            Assert.Equal(3, diffs[0]["Offset"]);

            // The difference has a length of 1 characters
            Assert.Equal(1, diffs[0]["Length"]);
        }
    }
}
