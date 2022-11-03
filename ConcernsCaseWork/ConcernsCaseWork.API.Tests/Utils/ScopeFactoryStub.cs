using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.API.Tests.Utils
{
    public class ScopeFactoryStub : IServiceScopeFactory
    {
        private readonly object[] _services;

        public ScopeFactoryStub(params object[] services)
        {
            _services = services;
        }

        public IServiceScope CreateScope()
        {
            return new ScopeStub { ServiceProvider = new ServiceProviderStub(_services) };
        }

        private class ScopeStub : IServiceScope
        {
            public IServiceProvider ServiceProvider { get; set; }

            public void Dispose() { }
        }

        private class ServiceProviderStub : IServiceProvider
        {
            private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

            public ServiceProviderStub(object[] services)
            {
                foreach (var service in services)
                {
                    _services.Add(service.GetType(), service);
                }
            }

            public object GetService(Type serviceType)
            {
                foreach (var service in _services)
                {
                    if (serviceType.IsAssignableFrom(service.Key))
                    {
                        return service.Value;
                    }
                }
                return null;
            }
        }
    }
}
