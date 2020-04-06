namespace Injecter
{
    public sealed class InjecterOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether during injection cache the fields, properties, methods needing injection.
        /// </summary>
        public bool UseCaching { get; set; } = true;
    }
}
