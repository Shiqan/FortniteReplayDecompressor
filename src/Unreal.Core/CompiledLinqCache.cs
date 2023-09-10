using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unreal.Core.Contracts;

namespace Unreal.Core;

public sealed class CompiledLinqCache
{
    private readonly Dictionary<Type, Func<IProperty>> _compiledIPropertyBuilders = new();

    private readonly KeyList<Type, Func<INetFieldExportGroup>> _compiledBuilders = new();

    internal int TotalTypes => _compiledBuilders.Length;

    internal int AddExportType(Type type)
    {
        if (_compiledBuilders.TryGetIndex(type, out var index))
        {
            return index;
        }

        _compiledBuilders.Add(type, CreateFunction<INetFieldExportGroup>(type));

        return _compiledBuilders.Length - 1;
    }

    public INetFieldExportGroup CreateObject(int typeId) => _compiledBuilders[typeId]();


    public IProperty CreatePropertyObject(Type type)
    {
        if (_compiledIPropertyBuilders.TryGetValue(type, out var builder))
        {
            return builder();
        }

        builder = CreateFunction<IProperty>(type);
        _compiledIPropertyBuilders[type] = builder;

        return builder();
    }

    private static Func<T> CreateFunction<T>(Type type)
    {
        var block = Expression.Block(type, Expression.New(type));
        var builder = Expression.Lambda<Func<T>>(block).Compile();

        return builder;
    }
}