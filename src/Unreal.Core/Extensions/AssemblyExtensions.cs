using System.Reflection;

namespace Unreal.Core.Extensions;

public static class AssemblyExtensions
{
    public static IEnumerable<Type> GetAllWithInterface(this Assembly assembly, Type @interface) => assembly.GetTypes().Where(t => t.IsClass && t.IsAbstract == false && t.GetInterfaces().Contains(@interface));
    public static IEnumerable<Type> GetAllWithInterface<TType>(this Assembly assembly) => assembly.GetAllWithInterface(typeof(TType));
    public static IEnumerable<Type> GetAllWithAttribute<TType, TAttribute>(this Assembly assembly) => assembly.GetAllWithInterface<TType>().Where(t => t.GetCustomAttribute(typeof(TAttribute)) is not null);
}
