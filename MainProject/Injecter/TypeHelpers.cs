using System;
using System.Collections.Generic;
using System.Reflection;

namespace Injecter
{
    internal static class TypeHelpers
    {
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
