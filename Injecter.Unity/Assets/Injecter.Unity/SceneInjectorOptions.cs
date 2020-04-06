namespace Injecter.Unity
{
    public sealed class SceneInjectorOptions
    {
        public enum Behavior
        {
            Factory,
            CompositionRoot,
        }

        public bool DontDestroyOnLoad { get; set; } = true;
        public Behavior InjectionBehavior { get; set; } = Behavior.Factory;
    }
}
