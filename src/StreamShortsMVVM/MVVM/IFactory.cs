using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamShorts.MVVM.MVVM
{
    public interface IFactory<T>
    {
        T? Create();
        T? Create(string key);
    }
}
