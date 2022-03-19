using System;

namespace MuonhoryoLibrary
{
    /// <summary>
    /// Before getting result from OneUseAlgorithm needed execute ExecuteAlgorithm().
    /// </summary>
    /// <typeparam name="TResultType"></typeparam>
    public abstract class OneUseAlgorithm<TResultType>
    {
        protected enum OneUseAlgorithmState
        {
            NotBeenUsed,
            PathFindInProgress,
            BeenUsed
        }
        protected OneUseAlgorithmState CurrentState=OneUseAlgorithmState.NotBeenUsed;
        /// <summary>
        /// After end of algorithm CurrentState must be BeenUsed.
        /// </summary>
        protected abstract void StartAlgorithm();
        /// <summary>
        /// If algorithm has been used or is executed,throw error.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void ExecuteAlgorithm()
        {
            if (CurrentState == OneUseAlgorithmState.NotBeenUsed)
            {
                CurrentState = OneUseAlgorithmState.PathFindInProgress;
                ExecuteAlgorithm();
            }
            else
            {
                throw new Exception("Algorithm must be unused for this method.");
            }
        }
        protected abstract TResultType ReturnResult();
        /// <summary>
        /// If algorithm hasn't been used or is executed,throw error.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TResultType GetResult()
        {
            if (CurrentState == OneUseAlgorithmState.BeenUsed)
            {
                return ReturnResult();
            }
            else
            {
                throw new Exception("Algorithm must be used for this method.");
            }
        }
    }
}
