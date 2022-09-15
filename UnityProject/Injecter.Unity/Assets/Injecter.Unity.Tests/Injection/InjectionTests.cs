using Injecter.Unity.Tests.Arrange.ComplexSceneInjectedProperly;
using Injecter.Unity.Tests.Arrange.DisposeIsCalledOnDestroy;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

using Object = UnityEngine.Object;

namespace Injecter.Unity.Tests.Injection
{
    public sealed class InjectionTests
    {
        [UnityTest]
        public IEnumerator ComplexSceneInjectedProperly()
        {
            // Arrange
            CompositionRoot.ServiceProvider = new ServiceCollection()
                .AddInjecter()
                .AddTransient<TestService>()
                .BuildServiceProvider();
            SceneManager.LoadScene($"Injecter.Unity.Tests/Scenes/{nameof(ComplexSceneInjectedProperly)}", LoadSceneMode.Single);

            // Act
            yield return new WaitForFixedUpdate();

            // Assert
            var sut = Object.FindObjectOfType<ComplexConcrete>();
            sut.AssertBaseInjected();
            sut.AssertConcreteInjected();
        }

        [UnityTest]
        public IEnumerator DisposeIsCalledOnDestroy()
        {
            // Arrange
            CompositionRoot.ServiceProvider = new ServiceCollection()
                .AddInjecter()
                .AddSingleton<ListenToDispose>()
                .AddTransient<TestDisposable>()
                .BuildServiceProvider();

            SceneManager.LoadScene($"Injecter.Unity.Tests/Scenes/{nameof(DisposeIsCalledOnDestroy)}", LoadSceneMode.Single);

            // Act
            yield return new WaitForFixedUpdate();

            var store = CompositionRoot.ServiceProvider.GetRequiredService<IScopeStore>();
            Assert.AreEqual(1, store.NumberOfScopes);

            var sut = GameObject.Find("SUT");
            Object.Destroy(sut);

            yield return new WaitForFixedUpdate();

            // Assert
            var listener = CompositionRoot.ServiceProvider.GetRequiredService<ListenToDispose>();
            //var store = CompositionRoot.ServiceProvider.GetRequiredService<IScopeStore>();
            Assert.True(listener.DisposeCalled);
            Assert.AreEqual(0, store.NumberOfScopes);
        }

        [TearDown]
        public void Cleanup() => (CompositionRoot.ServiceProvider as IDisposable)?.Dispose();
    }
}
