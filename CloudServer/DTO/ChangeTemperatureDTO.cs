using CloudServer.Validators;
using System.ComponentModel.DataAnnotations;

namespace CloudServer.DTO
{
    public class ChangeTemperatureDTO
    {
        [Range(16, 30)]
        public decimal Degree { get; set; }

        [AfterDate]
        public DateTime Date { get; set; }
    }
}
