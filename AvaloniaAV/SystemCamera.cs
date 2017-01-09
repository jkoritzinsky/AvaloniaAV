namespace AvaloniaAV
{
    public class SystemCamera
    {
        public SystemCamera(string friendlyName, string systemIdentifier)
        {
            FriendlyName = friendlyName;
            SystemIdentifier = systemIdentifier;
        }
        public string FriendlyName { get; }
        public string SystemIdentifier { get; }
    }
}
