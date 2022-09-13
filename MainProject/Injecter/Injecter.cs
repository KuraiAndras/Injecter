using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Injecter
{
    public sealed class Injecter : IInjecter
    {
        private readonly ConcurrentDictionary<Type, InjectableMembers> _resolveDictionary = new();

        private readonly InjecterOptions _options;
        private readonly IScopeStore _scopeStore;
        private readonly IServiceProvider _serviceProvider;

        public Injecter(IServiceProvider serviceProvider, InjecterOptions options, IScopeStore scopeStore)
        {
            _serviceProvider = serviceProvider;
            _options = options;
            _scopeStore = scopeStore;
        }

        public IServiceScope? InjectIntoType(Type type, object instance, bool createScope)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (instance is null) throw new ArgumentNullException(nameof(instance));

            var members = GetMembers(type);
            if (!members.CanInject) return null;

            IServiceScope? serviceScope = createScope ? _scopeStore.CreateScope(instance) : null;
            var serviceProvider = createScope switch
            {
                true => serviceScope!.ServiceProvider,
                false => _serviceProvider,
            };

            InjectMany(instance, serviceProvider, members.Fields);
            InjectMany(instance, serviceProvider, members.Properties);
            InjectMany(instance, serviceProvider, members.Methods);

            return serviceScope;
        }

        public IServiceScope? InjectIntoType<T>(T instance, bool createScope)
            where T : notnull
            => InjectIntoType(typeof(T), instance, createScope);

        private static void InjectMany(object instance, IServiceProvider serviceProvider, MemberInfo[] memberInfos)
        {
            for (var i = 0; i < memberInfos.Length; i++)
            {
                Inject(instance, serviceProvider, memberInfos[i]);
            }
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

        private InjectableMembers GetMembers(Type type) =>
            !_options.UseCaching
                ? InjectableMembers.Create(type)
                : _resolveDictionary.GetOrAdd(type, t => InjectableMembers.Create(t));
    }
}
