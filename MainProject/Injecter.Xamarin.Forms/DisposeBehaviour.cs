namespace Injecter.Xamarin.Forms
{
    public enum DisposeBehaviour
    {
        /// <summary>
        /// Dispose when the window containing the control is closed.
        /// </summary>
        OnDisappearing,

        /// <summary>
        /// Don't dispose automatically.
        /// </summary>
        Manual,
    }
}
