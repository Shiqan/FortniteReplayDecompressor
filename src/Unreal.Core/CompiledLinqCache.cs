using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Unreal.Core
{
    public class CompiledLinqCache
    {
        private Dictionary<Type, Func<dynamic>> _compiledBuilders = new();

        public object CreateObject(Type type)
        {
            if (_compiledBuilders.TryGetValue(type, out Func<dynamic> builder))
            {
                return builder();
            }

            var block = Expression.Block(type, Expression.New(type));
            builder = Expression.Lambda<Func<dynamic>>(block).Compile();

            _compiledBuilders[type] = builder;

            return builder();
        }
    }
}