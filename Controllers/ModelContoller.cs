using Microsoft.AspNetCore.Mvc;
using MLTEST;
using NetPract3.Service;

namespace NetPract3.Controllers
{
    [ApiController]
    [Route("api")]
    public class ModelContoller : ControllerBase
    {
        private readonly ModelService _service;

        public ModelContoller(ModelService service)
        {
            _service = service;
        }

        [HttpPost("train")]
        public IActionResult Train()
        {
            return Ok(_service.Train());
        }

        [HttpPost("predict")]
        public IActionResult Predict(Mushroom data)
        {
            try
            {
                Console.WriteLine("Got predict on controller");

                return Ok(_service.Predict(data));

            } catch (Exception ex)
            {
                return StatusCode(500, $"Error has been occured while predicting new entity:{ex.Message}");
            }
        }

        [HttpPost("retrain")]
        public IActionResult Retrain(Mushroom data)
        {
            try
            {
                _service.Retrain(data);
                return Ok("Model was retrained with new data succesfully");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error has been occured while retraining with the new entity:{ex.Message}");
            }
        }

    }
}
