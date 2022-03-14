﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace StudentManager
{
    public class FooFactory
    {
        readonly IServiceProvider _serviceProvider;

        public FooFactory(IServiceProvider serviceProvider)
        {
            // Somehow this seems to get the IsRootProvider=true instance...
            _serviceProvider = serviceProvider;
        }

        public IFoo Get() => true 
            ? _serviceProvider.GetService<FooA>() 
            : _serviceProvider.GetService<FooB>();
    }

    public interface IFoo
    {

    }

    public class FooA : IFoo
    {

    }

    public class FooB : IFoo
    {

    }
}
