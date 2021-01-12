using System;

namespace Injecter.Unity
{
    public class ComponentMissingException : Exception
    {
        public ComponentMissingException(string containingGameObject) => ContainingGameObject = containingGameObject;

        public string ContainingGameObject { get; }
    }
}
