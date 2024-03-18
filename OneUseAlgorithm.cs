
using System;

namespace MuonhoryoLibrary
{
    /// <summary>
    /// Before getting result from OneUseAlgorithm needed execute ExecuteAlgorithm().
    /// </summary>
    /// <typeparam name="TResultType"></typeparam>
    public abstract class OneUseAlgorithm<TResultType>
    {
        public enum OneUseAlgorithmState
        {
            NotBeenUsed,
            AlgorithmInProgress,
            BeenUsed
        }
        public event Action<TResultType> OnEndExecuting;
        public OneUseAlgorithmState CurrentState { get; private set; } = OneUseAlgorithmState.NotBeenUsed;

        /// <summary>
        /// If algorithm has been used or is executed,throw error.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void ExecuteAlgorithm()
        {
            if (CurrentState == OneUseAlgorithmState.NotBeenUsed)
            {
                CurrentState = OneUseAlgorithmState.AlgorithmInProgress;
                ExecuteAlgorithm();
            }
            else
            {
                throw new Exception("Algorithm has been used or is in execution progress.");
            }
        }

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
                throw new Exception("Haven't result. Algorithm hasn't been used.");
            }
        }


        protected abstract void StartAlgorithm();
        protected void EndAlgorithmRunning()
        {
            CurrentState = OneUseAlgorithmState.BeenUsed;
            OnEndExecuting?.Invoke(ReturnResult());
        }
        protected abstract TResultType ReturnResult();
    }
}
