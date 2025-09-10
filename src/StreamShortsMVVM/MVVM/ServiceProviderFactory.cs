using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamShorts.MVVM.MVVM
{

  public class ServiceProviderFactory<T> : IFactory<T> where T : class
  {
    public ServiceProviderFactory(IServiceProvider serviceProvider, ILogger<T> logger)
    {
      _serviceProvider = serviceProvider;
      _logger = logger;
    }
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<T> _logger;

    public T? Create()
    {
      T? instance = _serviceProvider.GetService<T>();
      return instance;
    }
    public T? Create(string key)
    {
      try
      {
        return _serviceProvider.GetKeyedService<T>(key);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error creating instance of {Type} with key {Key}", typeof(T).FullName, key);
        throw;
      }
    }
  }
}
