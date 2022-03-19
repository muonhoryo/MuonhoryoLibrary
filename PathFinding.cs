using System;
using System.Collections.Generic;
using MuonhoryoLibrary.Exceptions;

namespace MuonhoryoLibrary.PathFinding2D
{
    public class TwoSidedWay
    {
        private TwoSidedWay() { }
        public TwoSidedWay(ITwoSidedPathPoint FirstPoint, ITwoSidedPathPoint SecondPoint, float WayLength)
        {
            if (FirstPoint == SecondPoint)
            {
                throw new ArgumentException("Points must be different.");
            }
            this.FirstPoint = FirstPoint??throw new ArgumentNullException("FirstPoint".NullRefExceptionText());
            this.SecondPoint = SecondPoint??throw new ArgumentNullException("SecondPoint".NullRefExceptionText());
            FirstPoint.AddWay(this);
            SecondPoint.AddWay(this);
            this.WayLength = WayLength>0?WayLength:throw new ArithmeticException("WayLength must be more then 0.");
        }
        public readonly float WayLength;
        public readonly ITwoSidedPathPoint FirstPoint;
        public readonly ITwoSidedPathPoint SecondPoint;
    }
    public interface ITwoSidedPathPoint
    {
        public float X { get; }
        public float Y { get; }
        public IEnumerable<TwoSidedWay> Ways { get; }
        public void AddWay(TwoSidedWay way);
        public void RemoveWay(TwoSidedWay way);
    }
    public sealed class DextraAlgorithm:OneUseAlgorithm<ITwoSidedPathPoint[]>
    {
        private sealed class DextraPath
        {
            private DextraPath() { }
            public DextraPath(ITwoSidedPathPoint start)
            {
                Path = new List<ITwoSidedPathPoint> { start };
                Length = 0;
            }
            public DextraPath(List<ITwoSidedPathPoint> Path, float Length)
            {
                this.Path = Path;
                this.Length = Length;
            }
            public DextraPath(DextraPath copyiedPath)
            {
                Path = new List<ITwoSidedPathPoint> { };
                Path.AddRange(copyiedPath.Path);
                Length = copyiedPath.Length;
            }
            public readonly List<ITwoSidedPathPoint> Path;
            public ITwoSidedPathPoint LastPoint => Path[Path.Count - 1];
            public readonly float Length;
        }
        private DextraAlgorithm() { }
        public DextraAlgorithm(ITwoSidedPathPoint Start, ITwoSidedPathPoint End)
        {
            if (Start == null)
            {
                throw new ArgumentNullException("Start point".NullRefExceptionText());
            }
            this.End = End ?? throw new ArgumentNullException("End point".NullRefExceptionText());
            Paths = new List<DextraPath> { new DextraPath(Start) };
        }
        private readonly ITwoSidedPathPoint End;
        private ITwoSidedPathPoint[] FinalPathWay = null;
        private readonly List<ITwoSidedPathPoint> CheckedPathpoints = new List<ITwoSidedPathPoint> { };
        private readonly List<DextraPath> Paths;
        private int IndexOfLastPoint(ITwoSidedPathPoint point)
        {
            for (int i = 0; i < Paths.Count; i++)
            {
                if (Paths[i].LastPoint == point)
                {
                    return i;
                }
            }
            return -1;
        }
        private void CheckPoints()
        {
            {
                Paths.Sort(delegate (DextraPath x, DextraPath y)
                {
                    if (x.Length > y.Length)
                    {
                        return 1;
                    }
                    else if (x.Length == y.Length)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                });
                DextraPath[] pathsArray = Paths.ToArray();
                foreach (var path in pathsArray)
                {
                    if (path.LastPoint == End)
                    {
                        continue;
                    }
                    foreach (TwoSidedWay way in path.LastPoint.Ways)
                    {
                        ITwoSidedPathPoint endPoint = way.FirstPoint == path.LastPoint ? way.SecondPoint : way.FirstPoint;
                        if (CheckedPathpoints.Contains(endPoint))
                        {
                            continue;
                        }
                        int index = IndexOfLastPoint(endPoint);
                        if (index > -1)
                        {
                            float length=path.Length + way.WayLength;
                            if (length < Paths[index].Length)
                            {
                                Paths[index] = new DextraPath(path);
                                Paths[index].Path.Add(endPoint);
                            }
                        }
                        else
                        {
                            Paths.Add(new DextraPath(path));
                            Paths[Paths.Count - 1].Path.Add(endPoint);
                        }
                    }
                    Paths.Remove(path);
                    CheckedPathpoints.Add(path.LastPoint);
                }
            }
            if (Paths.Count == 0)
            {
                CurrentState = OneUseAlgorithmState.BeenUsed;
                FinalPathWay = null;
            }
            else if (Paths.Count == 1 && Paths[0].LastPoint == End)
            {
                CurrentState = OneUseAlgorithmState.BeenUsed;
                FinalPathWay = Paths[0].Path.ToArray();
            }
            else
            {
                CheckPoints();
            }
        }
        protected override void StartAlgorithm()
        {
            CheckPoints();
        }
        protected override ITwoSidedPathPoint[] ReturnResult()
        {
            return FinalPathWay;
        }
    }
}
