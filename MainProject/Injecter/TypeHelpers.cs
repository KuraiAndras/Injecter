using System;
using System.Collections.Generic;
using System.Reflection;

namespace Injecter
{
    public static class TypeHelpers
    {
        private const BindingFlags InstanceBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        public static IReadOnlyList<MemberInfo> GetInjectableMembers(Type targetType)
        {
            var allTypesInternal = targetType.GetAllTypes();

            var infos = new List<MemberInfo>();

            for (var i = 0; i < allTypesInternal.Count; i++)
            {
                var type = allTypesInternal[i];

                var fields = type.GetFields(InstanceBindingFlags);
                for (var j = 0; j < fields.Length; j++)
                {
                    var field = fields[j];
                    if (field.GetCustomAttribute<InjectAttribute>() != null) infos.Add(field);
                }

                var properties = type.GetProperties(InstanceBindingFlags);
                for (var j = 0; j < properties.Length; j++)
                {
                    var property = properties[j];
                    if (property.GetCustomAttribute<InjectAttribute>() != null) infos.Add(property);
                }

                var methods = type.GetMethods(InstanceBindingFlags);
                for (var j = 0; j < methods.Length; j++)
                {
                    var method = methods[j];
                    if (method.GetCustomAttribute<InjectAttribute>() != null) infos.Add(method);
                }
            }

            return infos;
        }

        internal static bool IsAutoProperty(this PropertyInfo property)
        {
            var fields = property.DeclaringType?.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            if (fields != null)
            {
                for (var i = 0; i < fields.Length; i++)
                {
                    if (fields[i].Name.Contains("<" + property.Name + ">"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static FieldInfo GetAutoPropertyBackingField(this PropertyInfo property)
        {
            var fields = property.DeclaringType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            for (var i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.Name.Contains("<" + property.Name + ">")) return field;
            }

            throw new InvalidOperationException($"Could not find auto property backing filed: {property.Name} on type: {property.DeclaringType}");
        }

        internal static IReadOnlyList<Type> GetAllTypes(this Type type)
        {
            var types = new List<Type>();

            Type currentType = type;

            while (true)
            {
                types.Add(currentType);

                if (currentType.BaseType == typeof(object) || currentType.BaseType == null)
                {
                    break;
                }

                currentType = currentType.BaseType;
            }

            return types;
        }
    }
}
