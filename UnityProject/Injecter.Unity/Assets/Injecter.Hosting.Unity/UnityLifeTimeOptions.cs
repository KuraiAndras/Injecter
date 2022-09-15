#nullable enable
namespace Injecter.Hosting.Unity
{
    public sealed class UnityLifeTimeOptions
    {
        /// <summary>
        /// Indicates if host lifetime status messages should be suppressed such as on startup.
        /// The default is false.
        /// </summary>
        public bool SuppressStatusMessages { get; set; }
    }
}
