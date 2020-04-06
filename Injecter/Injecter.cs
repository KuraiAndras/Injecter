using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Injecter
{
    public sealed class Injecter : IInjecter
    {
        private readonly ConcurrentDictionary<Type, (FieldInfo[] fieldInfos, PropertyInfo[] propertyInfos, MethodInfo[] methodInfos)> _resolveDictionary =
            new ConcurrentDictionary<Type, (FieldInfo[], PropertyInfo[], MethodInfo[])>();

        private readonly InjecterOptions _options;
        private readonly IServiceProvider _serviceProvider;

        public Injecter(IServiceProvider serviceProvider, InjecterOptions options)
        {
            _serviceProvider = serviceProvider;
            _options = options;
        }

        public IServiceScope InjectIntoType(Type type, object instance)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (instance is null) throw new ArgumentNullException(nameof(instance));

            var (fieldInfos, propertyInfos, methodInfos) = GetMembers(type);

            var scope = _serviceProvider.CreateScope();
            var didInstantiate = false;

            fieldInfos.ForEach(f => didInstantiate = Inject(instance, scope, f));
            propertyInfos.ForEach(p => didInstantiate = Inject(instance, scope, p));
            methodInfos.ForEach(m => didInstantiate = Inject(instance, scope, m));

            return didInstantiate ? scope : null;
        }

        public IServiceScope InjectIntoType<T>(T instance) => InjectIntoType(typeof(T), instance);

        private static bool Inject(object instance, IServiceScope scope, MemberInfo memberInfo)
        {
            object GetService(IServiceScope scopeInternal, Type memberTypeInternal) => scopeInternal.ServiceProvider.GetService(memberTypeInternal);

            switch (memberInfo)
            {
                case FieldInfo field:
                {
                    field.SetValue(instance, GetService(scope, field.FieldType));

                    return true;
                }
                case PropertyInfo property:
                {
                    if (property.CanWrite)
                    {
                        property.SetValue(instance, GetService(scope, property.PropertyType));
                        return true;
                    }

                    if (!property.IsAutoProperty()) return false;

                    property.GetAutoPropertyBackingField().SetValue(instance, GetService(scope, property.PropertyType));

                    return true;
                }
                case MethodInfo method:
                {
                    if (method.IsConstructor) return false;

                    var methodParameters = method.GetParameters();
                    var parameters = new object[methodParameters.Length];
                    for (var i = 0; i < methodParameters.Length; i++)
                    {
                        parameters[i] = GetService(scope, methodParameters[i].ParameterType);
                    }

                    method.Invoke(instance, parameters);

                    return true;
                }
                default: throw new MemberAccessException($"Unknown member: {memberInfo}");
            }
        }

        private (FieldInfo[] fieldInfos, PropertyInfo[] propertyInfos, MethodInfo[] methodInfos) GetMembers(Type type)
        {
            (FieldInfo[] fields, PropertyInfo[] properties, MethodInfo[] methods) GetMembersInternal(IReadOnlyCollection<Type> allTypesInternal)
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
}
