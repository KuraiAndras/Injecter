#nullable enable
using System;

namespace Injecter.Unity
{
#pragma warning disable RCS1194 // Implement exception constructors.
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class ComponentMissingException : Exception
    {
        public ComponentMissingException(string containingGameObject) => ContainingGameObject = containingGameObject;

        public string ContainingGameObject { get; }
    }
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
#pragma warning restore RCS1194 // Implement exception constructors.
}
