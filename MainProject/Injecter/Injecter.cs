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

        public IServiceScope? InjectIntoType(Type type, object instance, bool createScope = true)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (instance is null) throw new ArgumentNullException(nameof(instance));

            var (fieldInfos, propertyInfos, methodInfos) = GetMembers(type);

            IServiceScope? serviceScope = createScope ? _serviceProvider.CreateScope() : null;
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

        public IServiceScope? InjectIntoType<T>(T instance, bool createScope = true)
            where T : notnull
            => InjectIntoType(typeof(T), instance, createScope);

        private static void Inject(object instance, IServiceProvider serviceProvider, MemberInfo memberInfo)
        {
            static object GetService(IServiceProvider serviceProvider, Type memberTypeInternal) => serviceProvider.GetService(memberTypeInternal);

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
