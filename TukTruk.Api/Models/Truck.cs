using TukTruk.Api.Enums;

namespace TukTruk.Api.Models
{
    public class Truck
    {
        public Guid? Id { get; set; }
        public TruckModelEnum Model { get; set; }
        public int ManufacturingYear { get; set; }
        public int ModelYear { get; set; }
    }
}