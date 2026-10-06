namespace EyeProtect.Models
{
    public class BreakSkippedNotificationContent
    {
        public BreakSkippedNotificationContent(string message, bool isFullscreen)
        {
            Message = message;
            IsFullscreen = isFullscreen;
        }

        public string Message { get; }

        public bool IsFullscreen { get; }
    }
}
