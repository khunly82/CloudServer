using CloudServer.DTO;
using CloudServer.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CloudServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThermostatController : ControllerBase
    {
        private readonly MqttConnection _mqttConnection;

        public ThermostatController(MqttConnection mqttConnection)
        {
            _mqttConnection = mqttConnection;
        }

        [HttpPost]
        public async Task<IActionResult> ChangeTemperature(ChangeTemperatureDTO data)
        {
            bool success = await _mqttConnection.PublishAsync(
                // A qui ?
                "testTF",
                // Quelles infos ?
                data.Degree.ToString()
            );
            if (success)
            {
                return Ok();
            }
            return Problem("La connection au thermostat a échoué");
        }
    }
}
