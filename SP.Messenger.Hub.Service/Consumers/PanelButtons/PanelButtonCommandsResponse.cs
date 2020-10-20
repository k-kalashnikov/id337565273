namespace SP.Consumers.Models
{
    public class PanelButtonCommandsResponse
    {
        public bool Success { get; set; }

        public static PanelButtonCommandsResponse Create(bool success)
            => new PanelButtonCommandsResponse
            {
                Success = success
            };
    }
}