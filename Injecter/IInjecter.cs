using Microsoft.Extensions.DependencyInjection;
using System;

namespace Injecter
{
    public interface IInjecter
    {
        IServiceScope InjectIntoType(Type type, object instance);
    }
}