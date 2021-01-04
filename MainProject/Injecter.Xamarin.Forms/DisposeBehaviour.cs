namespace Injecter.Xamarin.Forms
{
    public enum DisposeBehaviour
    {
        /// <summary>
        /// Dispose when parent is set to null in OnParentSet.
        /// </summary>
        OnNoParent,

        /// <summary>
        /// Don't dispose automatically.
        /// </summary>
        Manual,
    }
}
