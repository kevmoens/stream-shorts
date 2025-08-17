using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamShorts.MVVM.MVVM
{

    public class ServiceProviderFactory<T> : IFactory<T> where T : class
    {
        public ServiceProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;            
        }
        private readonly IServiceProvider _serviceProvider;


        public T? Create()
        {
            T? instance = _serviceProvider.GetService<T>();
            return instance;
        }
        public T? Create(string key)
        {
            return _serviceProvider.GetKeyedService<T>(key);
        }
    }
}
