namespace Injecter.Uwp
{
    public enum DisposeBehaviour
    {
        /// <summary>
        /// Dispose when the window containing the control is closed.
        /// </summary>
        OnWindowClose,

        /// <summary>
        /// Dispose when the control is unloaded. It is useful for ListViewItems and similar.
        /// </summary>
        OnUnloaded,

        /// <summary>
        /// Don't dispose automatically.
        /// </summary>
        Manual,
    }
}
