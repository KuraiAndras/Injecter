using Injecter.Unity.Tests.Arrange.ComplexSceneInjectedProperly;
using Injecter.Unity.Tests.Arrange.DisposeIsCalledOnDestroy;
using Injecter.Unity.Tests.Arrange.InjectorAwakeCalledFirst;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Injecter.Unity.Tests.Injection
{
    public sealed class InjectionTests
    {
        [UnityTest]
        public IEnumerator InjectorAwakeCalledFirst()
        {
            // Arrange
            SceneManager.LoadScene($"Injecter.Unity.Tests/Scenes/{nameof(InjectorAwakeCalledFirst)}", LoadSceneMode.Single);

            // Act
            yield return null;

            // Assert
            var sut = Object.FindObjectOfType<AwakeLogger>();
            Assert.AreEqual(sut.CallerTypes.Count, 6);
        }

        [UnityTest]
        public IEnumerator ComplexSceneInjectedProperly()
        {
            // Arrange
            SceneManager.LoadScene($"Injecter.Unity.Tests/Scenes/{nameof(ComplexSceneInjectedProperly)}", LoadSceneMode.Single);

            // Act
            yield return null;

            // Assert
            var sut = Object.FindObjectOfType<ComplexConcrete>();
            sut.AssertBaseInjected();
            sut.AssertConcreteInjected();
        }

        [UnityTest]
        public IEnumerator DisposeIsCalledOnDestroy()
        {
            // Arrange
            SceneManager.LoadScene($"Injecter.Unity.Tests/Scenes/{nameof(DisposeIsCalledOnDestroy)}", LoadSceneMode.Single);

            // Act
            yield return null;

            var sut = GameObject.Find("SUT");
            Object.Destroy(sut);

            yield return null;

            // Assert
            var listener = Object.FindObjectOfType<ListenToDispose>();
            Assert.True(listener.DisposeCalled);
        }
    }
}
