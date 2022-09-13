using System;
using System.Linq;
using System.Reflection;

namespace Injecter
{
    public sealed class InjectableMembers
    {
        private const BindingFlags InstanceBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private InjectableMembers(Type classType, FieldInfo[] fields, PropertyInfo[] properties, MethodInfo[] methods)
        {
            ClassType = classType;
            Fields = fields;
            Properties = properties;
            Methods = methods;

            CanInject = Fields.Length != 0 || Properties.Length != 0 || Methods.Length != 0;
        }

        public Type ClassType { get; }
        public FieldInfo[] Fields { get; }
        public PropertyInfo[] Properties { get; }
        public MethodInfo[] Methods { get; }

        public bool CanInject { get; }

        public static InjectableMembers Create(Type type)
        {
            var allTypesInternal = type
                .GetAllTypes()
                .ToList();

            var fields = allTypesInternal
                .SelectMany(t => t.GetFields(InstanceBindingFlags))
                .GetInjectables();

            var properties = allTypesInternal
                .SelectMany(t => t.GetProperties(InstanceBindingFlags))
                .GetInjectables();

            var methods = allTypesInternal
                .SelectMany(t => t.GetMethods(InstanceBindingFlags))
                .GetInjectables();

            return new InjectableMembers(type, fields, properties, methods);
        }
    }
}
