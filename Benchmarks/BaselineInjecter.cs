using Injecter;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;

public sealed class BaselineInjecter : IInjecter
{
    private readonly ConcurrentDictionary<Type, (FieldInfo[] fieldInfos, PropertyInfo[] propertyInfos, MethodInfo[] methodInfos)> _resolveDictionary = new();

    private readonly InjecterOptions _options;
    private readonly IScopeStore _scopeStore;
    private readonly IServiceProvider _serviceProvider;

    public BaselineInjecter(IServiceProvider serviceProvider, InjecterOptions options, IScopeStore scopeStore)
    {
        _serviceProvider = serviceProvider;
        _options = options;
        _scopeStore = scopeStore;
    }

    public IServiceScope? InjectIntoType(Type type, object instance, bool createScope)
    {
        if (type is null) throw new ArgumentNullException(nameof(type));
        if (instance is null) throw new ArgumentNullException(nameof(instance));

        var (fieldInfos, propertyInfos, methodInfos) = GetMembers(type);

        IServiceScope? serviceScope = createScope ? _scopeStore.CreateScope(instance) : null;
        var serviceProvider = createScope switch
        {
            true => serviceScope!.ServiceProvider,
            false => _serviceProvider,
        };

        fieldInfos.ForEach(f => Inject(instance, serviceProvider, f));
        propertyInfos.ForEach(p => Inject(instance, serviceProvider, p));
        methodInfos.ForEach(m => Inject(instance, serviceProvider, m));

        return serviceScope;
    }

    public IServiceScope? InjectIntoType<T>(T instance, bool createScope)
        where T : notnull
        => InjectIntoType(typeof(T), instance, createScope);

    private static void Inject(object instance, IServiceProvider serviceProvider, MemberInfo memberInfo)
    {
        static object GetService(IServiceProvider serviceProvider, Type memberTypeInternal) => serviceProvider.GetService(memberTypeInternal)!;

        switch (memberInfo)
        {
            case FieldInfo field:
                {
                    field.SetValue(instance, GetService(serviceProvider, field.FieldType));

                    break;
                }
            case PropertyInfo property:
                {
                    if (property.CanWrite)
                    {
                        property.SetValue(instance, GetService(serviceProvider, property.PropertyType));
                    }

                    if (!property.IsAutoProperty()) break;

                    property.GetAutoPropertyBackingField().SetValue(instance, GetService(serviceProvider, property.PropertyType));

                    break;
                }
            case MethodInfo method:
                {
                    if (method.IsConstructor) break;

                    var methodParameters = method.GetParameters();
                    var parameters = new object[methodParameters.Length];
                    for (var i = 0; i < methodParameters.Length; i++)
                    {
                        parameters[i] = GetService(serviceProvider, methodParameters[i].ParameterType);
                    }

                    method.Invoke(instance, parameters);

                    break;
                }
            default: throw new MemberAccessException($"Unknown member: {memberInfo}");
        }
    }

    private (FieldInfo[] fieldInfos, PropertyInfo[] propertyInfos, MethodInfo[] methodInfos) GetMembers(Type type)
    {
        static (FieldInfo[] fields, PropertyInfo[] properties, MethodInfo[] methods) GetMembersInternal(IReadOnlyCollection<Type> allTypesInternal)
        {
            const BindingFlags instanceBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            var fieldsToInject = allTypesInternal
                .SelectMany(t => t.GetFields(instanceBindingFlags))
                .FilterMembersToArray();

            var propertiesToInject = allTypesInternal
                .SelectMany(t => t.GetProperties(instanceBindingFlags))
                .FilterMembersToArray();

            var methodsToInject = allTypesInternal
                .SelectMany(t => t.GetMethods(instanceBindingFlags))
                .FilterMembersToArray();

            return (fieldsToInject, propertiesToInject, methodsToInject);
        }

        var allTypes = type
            .GetAllTypes()
            .ToList();

        if (!_options.UseCaching) return GetMembersInternal(allTypes);

        if (!_resolveDictionary.TryGetValue(type, out var members))
        {
            members = GetMembersInternal(allTypes);
            _resolveDictionary.TryAdd(type, members);
        }

        return members;
    }
}


public static class InjecterExtensions
{
    internal static T[] FilterMembersToArray<T>(this IEnumerable<T> members)
        where T : MemberInfo =>
        members
            .Where(m => m.GetCustomAttributes<InjectAttribute>().Any())
            .Distinct()
            .ToArray();

    internal static IEnumerable<Type> GetAllTypes(this Type type)
    {
        yield return type;

        foreach (var parentType in type.GetParentTypes())
        {
            yield return parentType;
        }
    }

    private static IEnumerable<Type> GetParentTypes(this Type type)
    {
        if (type == typeof(object) || type.BaseType() == typeof(object))
        {
            yield break;
        }

        yield return type.BaseType();

        foreach (var ancestor in type.BaseType().GetParentTypes())
        {
            yield return ancestor;
        }
    }

    private static Type BaseType(this Type type) => type.GetTypeInfo().BaseType!;

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

    internal static void ForEach<T>(this T[] source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }
}
