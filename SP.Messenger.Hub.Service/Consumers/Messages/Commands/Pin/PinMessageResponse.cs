namespace SP.Consumers.Models
{
    public class PinMessageResponse
    {
        public bool Success { get; set; }

        public static PinMessageResponse Create(bool success)
            => new PinMessageResponse
            {
                Success = success
            };
    }
}