using System.Reflection;
using Unreal.Core.Contracts;

namespace Unreal.Core.Options;

public class NetFieldParserOptions
{
    public List<Type> NetFieldExportGroups { get; set; } = new();

    /// <summary>
    /// Add <paramref name="type"/> to <see cref="NetFieldExportGroups"/>.
    /// </summary>
    public void AddNetFieldExportGroup(Type type) => NetFieldExportGroups.Add(type);

    /// <summary>
    /// Add type of <typeparamref name="T"/> to <see cref="NetFieldExportGroups"/>.
    /// </summary>
    public void AddNetFieldExportGroup<T>() where T : class, INetFieldExportGroup => AddNetFieldExportGroup(typeof(T));

    /// <summary>
    /// Add <paramref name="types"/> to <see cref="NetFieldExportGroups"/>.
    /// </summary>
    public void AddNetFieldExportGroups(IEnumerable<Type> types) => NetFieldExportGroups.AddRange(types);

    /// <summary>
    /// Add types of <typeparamref name="TMarker"/> assembly to <see cref="NetFieldExportGroups"/>.
    /// </summary>
    public void AddNetFieldExportGroupsFromAssembly<TMarker>() => AddNetFieldExportGroupsFromAssembly(typeof(TMarker).Assembly);

    /// <summary>
    /// Add types of <paramref name="assembly"/> to <see cref="NetFieldExportGroups"/>.
    /// </summary>
    public void AddNetFieldExportGroupsFromAssembly(Assembly assembly) => AddNetFieldExportGroups(assembly.GetTypes());
}
