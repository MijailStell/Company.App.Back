using Distributed.Services.Controllers;
using System.Linq;
using Xunit;

namespace Distributed.Services.Test
{
    public class UnitTest1
    {
        [Fact]
        public void GetMethod_WeatherForecast_ReturnSuccessfully()
        {
            WeatherForecastController controller = new();
            var returnValue = controller.Get() as WeatherForecast[];
            Assert.True(returnValue.Any());
        }
    }
}
