namespace Unreal.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// see UObjectBaseUtility
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RemoveAllPathPrefixes(this string path)
        {
            for (var i = path.Length - 1; i >= 0; i--)
            {
                switch (path[i])
                {
                    case '.':
                        return path.Substring(i + 1);
                    case '/':
                        return path;
                }
            }
            return path.RemovePathPrefix("Default__");
        }

        /// <summary>
        /// Remove the <paramref name="toRemove"/> string from the <paramref name="path"/>.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="toRemove"></param>
        /// <returns>string without given prefix</returns>
        public static string RemovePathPrefix(this string path, string toRemove)
        {
            if (toRemove.Length > path.Length)
            {
                return path;
            }

            for (var i = 0; i < toRemove.Length; i++)
            {
                if (path[i] != toRemove[i])
                {
                    return path;
                }
            }

            return path.Substring(toRemove.Length);
        }

        /// <summary>
        /// Clean given <paramref name="path"/> to get the base path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>string without additional number or underscore suffixes</returns>
        public static string CleanPathSuffix(this string path)
        {
            for (var i = path.Length - 1; i >= 0; i--)
            {
                if (!char.IsDigit(path[i]) && path[i] != '_')
                {
                    return path.Substring(0, i + 1);
                }
            }
            return path;
        }
    }
}
