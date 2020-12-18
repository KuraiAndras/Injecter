using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Injecter
{
    internal static class TypeHelpers
    {
        internal static bool IsAutoProperty(this PropertyInfo property) =>
            property
                .DeclaringType
                ?.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Any(f => f.Name.Contains("<" + property.Name + ">"))
            ?? false;

        internal static FieldInfo GetAutoPropertyBackingField(this PropertyInfo property) =>
            property.DeclaringType!
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(f => f.Name.Contains("<" + property.Name + ">"));

        internal static IEnumerable<Type> GetAllTypes(this Type type)
        {
            yield return type;

            foreach (var parentType in type.GetParentTypes())
            {
                yield return parentType;
            }
        }

        internal static T[] FilterMembersToArray<T>(this IEnumerable<T> members)
            where T : MemberInfo =>
            members
                .Where(m => m.GetCustomAttributes<InjectAttribute>().Any())
                .Distinct()
                .ToArray();

        internal static void ForEach<T>(this T[] source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        private static IEnumerable<Type> GetParentTypes(this Type type)
        {
            if (type == null || type.BaseType() == null || type == typeof(object) || type.BaseType() == typeof(object))
            {
                yield break;
            }

            yield return type.BaseType();

            foreach (var ancestor in type.BaseType().GetParentTypes())
            {
                yield return ancestor;
            }
        }

        private static Type BaseType(this Type type) => type.GetTypeInfo().BaseType;
    }
}
