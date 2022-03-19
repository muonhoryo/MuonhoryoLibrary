using System;

namespace MuonhoryoLibrary
{
    public static class Singltone
    {
        public static void Initialization<TSingltoneType>(ISingltone<TSingltoneType> script,Action destroyAction,
            Action initAction)
            where TSingltoneType : class
        {
            if (script.Singltone != null)
            {
                destroyAction();
            }
            else
            {
                script.Singltone = script as TSingltoneType;
                initAction();
            }
        }
    }
    /// <summary>
    /// Provide existance of once example of TSingltoneType with using Initialization() from Singltone static class.
    /// </summary>
    /// <typeparam name="TSingltoneType"></typeparam>
    public interface ISingltone<TSingltoneType> where TSingltoneType : class
    {
        public TSingltoneType Singltone { get; set; }
    }
}
