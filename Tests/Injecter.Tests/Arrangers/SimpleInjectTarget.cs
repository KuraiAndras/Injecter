﻿using Injecter.Tests.Arrangers.Services;

namespace Injecter.Tests.Arrangers
{
#pragma warning disable IDE0051 // Remove unused private members
    public sealed class SimpleInjectTarget
    {
        [Inject] private readonly ISimpleService? _simpleService = default;

        private ISimpleService? _simpleService2;

        [Inject] public ISimpleService? SimpleService { get; } = default;

        public bool IsServiceNotNull => !(_simpleService is null) && !(_simpleService2 is null);

        [Inject]
        private void Construct(ISimpleService simpleService) => _simpleService2 = simpleService;
    }
#pragma warning restore IDE0051 // Remove unused private members
}
