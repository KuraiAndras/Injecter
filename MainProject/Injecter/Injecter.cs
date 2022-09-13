using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Injecter
{
    public sealed class Injecter : IInjecter
    {
        private const BindingFlags InstanceBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private readonly ConcurrentDictionary<Type, IReadOnlyList<MemberInfo>> _resolveDictionary = new();

        private readonly InjecterOptions _options;
        private readonly IScopeStore _scopeStore;
        private readonly IServiceProvider _serviceProvider;

        public Injecter(IServiceProvider serviceProvider, InjecterOptions options, IScopeStore scopeStore)
        {
            _serviceProvider = serviceProvider;
            _options = options;
            _scopeStore = scopeStore;
        }

        public IServiceScope? InjectIntoType(object instance, bool createScope)
        {
            if (instance is null) throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();

            var members = !_options.UseCaching
                ? GetMembers(type)
                : _resolveDictionary.GetOrAdd(type, t => GetMembers(t));

            if (members.Count == 0) return null;

            IServiceScope? serviceScope = createScope ? _scopeStore.CreateScope(instance) : null;
            var serviceProvider = createScope switch
            {
                true => serviceScope!.ServiceProvider,
                false => _serviceProvider,
            };

            for (var i = 0; i < members.Count; i++)
            {
                Inject(instance, serviceProvider, members[i]);
            }

            return serviceScope;
        }

        private static void Inject(object instance, IServiceProvider serviceProvider, MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case FieldInfo field:
                    {
                        field.SetValue(instance, serviceProvider.GetService(field.FieldType));

                        break;
                    }
                case PropertyInfo property:
                    {
                        if (property.CanWrite)
                        {
                            property.SetValue(instance, serviceProvider.GetService(property.PropertyType));
                        }

                        if (!property.IsAutoProperty()) break;

                        property.GetAutoPropertyBackingField().SetValue(instance, serviceProvider.GetService(property.PropertyType));

                        break;
                    }
                case MethodInfo method:
                    {
                        if (method.IsConstructor) break;

                        var methodParameters = method.GetParameters();
                        var parameters = new object[methodParameters.Length];
                        for (var i = 0; i < methodParameters.Length; i++)
                        {
                            parameters[i] = serviceProvider.GetService(methodParameters[i].ParameterType);
                        }

                        method.Invoke(instance, parameters);

                        break;
                    }
                default: throw new MemberAccessException($"Unknown member: {memberInfo}");
            }
        }

        private static IReadOnlyList<MemberInfo> GetMembers(Type targetType)
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
    }
}
