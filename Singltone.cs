
using System;

namespace MuonhoryoLibrary
{
    public static class SingltoneInitializations
    {
        /// <summary>
        /// Initialize only first handled singltone. If singltone instance is already exists, destroy handled example,
        /// then execute initalize example with initAction
        /// </summary>
        /// <typeparam name="TSingltoneType"></typeparam>
        /// <param name="script"></param>
        /// <param name="destroyAction"></param>
        /// <param name="initAction"></param>
        public static void InitializationForFirstExample<TSingltoneType>(ISingltone<TSingltoneType> script,
            Action initAction)
            where TSingltoneType : class,ISingltone<TSingltoneType>
        {
            if (script.Singltone != null)
            {
                script.Destroy();
            }
            else
            {
                script.Singltone = script as TSingltoneType;
                initAction();
            }
        }

        /// <summary>
        /// Initialize example and set him as singltone instance. If singltone instance is already exists, destroy old instance.
        /// </summary>
        /// <typeparam name="TSingltoneType"></typeparam>
        /// <param name="script"></param>
        /// <param name="initAction"></param>
        public static void SubstituteInitialization<TSingltoneType>(ISingltone<TSingltoneType> script,Action initAction)
            where TSingltoneType:class, ISingltone<TSingltoneType>
        {
            if (script.Singltone != null)
                script.Singltone.Destroy();
            script.Singltone = script as TSingltoneType;
            initAction();
        }
    }
    public static class Singltone
    {
        private class MoreThanOneSingltonesExamplesException : Exception
        {
            public MoreThanOneSingltonesExamplesException(string scriptName)
                : base("Have more than one examples on " + scriptName) { }
        }
        /// <summary>
        /// Throw exception, if have more than one singltone examples.
        /// </summary>
        /// <typeparam name="TSingltoneType"></typeparam>
        /// <param name="owner"></param>
        /// <exception cref="MoreThanOneSingltonesExamplesException"></exception>
        public static void ValidateSingltone<TSingltoneType>(this TSingltoneType owner)
            where TSingltoneType : class, ISingltone<TSingltoneType>
        {
            if (owner.Singltone != null && owner.Singltone != owner)
            {
                throw new MoreThanOneSingltonesExamplesException(typeof(TSingltoneType).Name);
            }
            else
                owner.Singltone = owner;
        }
    }

    /// <summary>
    /// Provide existance of once example of TSingltoneType with using Initialization methods from SingltoneInitializations
    /// static class or custom initialization methods.
    /// </summary>
    /// <typeparam name="TSingltoneType"></typeparam>
    public interface ISingltone<TSingltoneType> where TSingltoneType : class,ISingltone<TSingltoneType>
    {
        public TSingltoneType Singltone { get; set; }
        public void Destroy();
    }

}
