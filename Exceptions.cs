
namespace MuonhoryoLibrary.Exceptions
{
    public static class Exceptions
    {
        /// <summary>
        /// Return string:"{targetName} cannot be null.".
        /// </summary>
        /// <param name="targetName"></param>
        /// <returns></returns>
        public static string NullRefExceptionText(this string targetName)
        {
            return string.Concat(targetName??"", " cannot be null.");
        }
    }
}
