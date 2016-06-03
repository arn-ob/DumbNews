namespace DumbNews.Lib.Utility
{
    using System;
    using System.Linq;
    using System.Reflection;

    public class ReflectionUtils
    {
        internal static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }
    }
}
