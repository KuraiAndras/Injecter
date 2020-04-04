using System;
using UnityEngine;

namespace Injecter.Unity
{
    /// <summary>
    /// Sets <see cref="CompositionRoot"/>
    /// Set Script Execution Order to early.
    /// </summary>
    public abstract class InjectStarter : MonoBehaviour
    {
        protected void Awake() => CompositionRoot.ServiceProvider = CreateServiceProvider();

        protected abstract IServiceProvider CreateServiceProvider();
    }
}
