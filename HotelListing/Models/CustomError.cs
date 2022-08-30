using Newtonsoft.Json;

namespace HotelListing.Models
{
    public class CustomError
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
