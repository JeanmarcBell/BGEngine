using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BGEngine.Hexagons
{
    public class HexGrid<T> : IEnumerable<T> where T : IHexGridObject
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Area { get; private set; }

        private T[,] grid;
        private static Dictionary<HexDirection, Point> neighbours = new Dictionary<HexDirection, Point>()
        {
            {HexDirection.NE, new Point(0, 1)},
            {HexDirection.NW, new Point(-1, 1)},
            {HexDirection.W, new Point(-1, 0)},
            {HexDirection.SW, new Point(-1, -1)},
            {HexDirection.SE, new Point(0, -1)},
            {HexDirection.E, new Point(1, 0)}
        };

        public HexGrid(int width, int height)
        {
            Width = width;
            Height = height;
            Area = width * height;
            grid = new T[width, height];
        }

        /// <summary>
        /// Returns an object given a Offset coordinate.
        /// </summary>
        public T this[int x, int y]
        {
            get
            {
                int wrappedY = y >= 0 ? (y % Height) : (Height + (y % Height));
                int wrappedX = x >= 0 ? (x % Width) : (Width + (x % Width));
                return grid[wrappedX, wrappedY];
            }
            set
            {
                int wrappedY = y >= 0 ? (y % Height) : (Height + (y % Height));
                int wrappedX = x >= 0 ? (x % Width) : (Width + (x % Width));
                grid[wrappedX, wrappedY] = value;
            }
        }

        public T this[Point point]
        {
            get { return this[point.X, point.Y]; }
        }

        /// <summary>
        /// Get a neighbour in a particular direction
        /// </summary>
        /// <param name="x">Horizontal index in the HexGrid of the center tile</param>
        /// <param name="y">Vertical index in the HexGrid of the center tile</param>
        /// <param name="direction"></param>
        public T Neighbour(int x, int y, HexDirection direction)
        {
            Point d = neighbours[direction];
            int offset = (d.Y == 0) ? 0 : y % 2;
            return this[x + d.X + offset, y + d.Y];
        }

        public T Neighbour(T obj, HexDirection direction)
        {
            return Neighbour(obj.HexGridLocation.X, obj.HexGridLocation.Y, direction);
        }

        /// <summary>
        /// Get a list of all neighbours surronding a tile
        /// </summary>
        /// <param name="x">Horizontal index in the HexGrid of the center tile</param>
        /// <param name="y">Vertical index in the HexGrid of the center tile</param>
        public List<T> Neighbours(int x, int y)
        {
            List<T> arr = new List<T>();
            foreach (HexDirection dir in neighbours.Keys)
            {
                arr.Add(Neighbour(x, y, dir));
            }
            return arr;
        }

        public List<T> Neighbours(Point point)
        {
            return Neighbours(point.X, point.Y);
        }

        public List<T> Neighbours(T hexObject)
        {
            return Neighbours(hexObject.HexGridLocation);
        }

        public HexDirection Direction(T start, T target)
        {
            int y = start.HexGridLocation.Y - target.HexGridLocation.Y; // y increases downwards
            int offset = y % 2 == 0 ? 0 : (start.HexGridLocation.Y % 2);
            int x = target.HexGridLocation.X - start.HexGridLocation.X - offset;
            Point p = new Point(x, y);

            var dir = neighbours.Single(kv => kv.Value == p).Key;
            return dir;
        }

        public Vector2 DirectionVector(T start, T target)
        {
            HexDirection dir = Direction(start, target);

            float angle = (int)dir * 60.0f * (float)(Math.PI / 180);
            Vector2 vec = Vector2Extensions.FromAngle(angle);
            vec.Y = -vec.Y;
            return vec;
        }

        public List<T> Range(int x, int y, int range)
        {
            List<T> results = new List<T>();
            Vector3 center = OffsetToCubed(x, y);

            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = Math.Max(-range, -dx - range); dy <= Math.Min(range, -dx + range); dy++)
                {
                    var dz = -dx - dy;
                    Vector3 cubed = new Vector3(dx + center.X, dy + center.Y, dz + center.Z);
                    Vector2 offsetVec = CubedToOffset(cubed);
                    results.Add(this[(int)offsetVec.X, (int)offsetVec.Y]);
                }
            }

            return results;
        }

        public List<T> ShortestPath(T start, T end)
        {
            Dictionary<T, int> costs = new Dictionary<T, int>();
            BinaryHeap<PathLocation> frontier = new BinaryHeap<PathLocation>();
            Dictionary<T, T> cameFrom = new Dictionary<T, T>();
            frontier.Push(new PathLocation(start, 0));
            costs[start] = 0;

            while (!frontier.IsEmpty())
            {
                var currentPathLoc = frontier.Pop();
                var current = currentPathLoc.HexGridObject;

                if (current.HexGridLocation == end.HexGridLocation)
                {
                    return BuildPathResult(start, cameFrom, current);
                }

                foreach (T next in Neighbours(current))
                {
                    int newCost = next.HexMovementCost + costs[current];
                    if (!costs.ContainsKey(next) || newCost < costs[next])
                    {
                        cameFrom[next] = current;
                        costs[next] = newCost;
                        int priority = newCost + Heuristic(next, end);
                        frontier.Push(new PathLocation(next, priority));
                    }
                }
            }

            return null;
        }

        private int Heuristic(T a, T b)
        {
            Point pointA = a.HexGridLocation;
            Point pointB = b.HexGridLocation;

            return Math.Abs(pointA.X - pointB.X) + Math.Abs(pointA.Y - pointB.Y);
        }

        private List<T> BuildPathResult(T start, Dictionary<T, T> cameFrom, T end)
        {
            List<T> result = new List<T>();
            T step = end;

            do
            {
                result.Add(step);
                step = cameFrom[step];
            } while (step.HexGridLocation != start.HexGridLocation);

            result.Add(step);
            result.Reverse();
            return result;
        }

        /// <summary>
        /// Get the unweighted distance between point a and b, where a & b are offset coordinates
        /// </summary>
        public int Distance(Point a, Point b)
        {
            return Distance(OffsetToCubed(a), OffsetToCubed(b));
        }

        public int Distance(Vector3 a, Vector3 b)
        {
            float max1 = Math.Max(Math.Abs(a.Y - b.Y), Math.Abs(a.Z - b.Z));
            return (int)Math.Max(Math.Abs(a.X - b.X), max1);
        }

        #region Conversion between coordinate systems
        // Converts odd-r offset to cubed coordinates
        public Vector3 OffsetToCubed(int x, int y)
        {
            Vector3 vec = new Vector3();
            vec.X = x - (y - (y & 1)) / 2;
            vec.Z = y;
            vec.Y = -vec.X - vec.Z;

            return vec;
        }

        public Vector3 OffsetToCubed(Point offset)
        {
            return OffsetToCubed(offset.X, offset.Y);
        }

        // Converts cubed coordinates to odd-r offsets
        public Vector2 CubedToOffset(int x, int y, int z)
        {
            Vector2 vec = new Vector2();
            vec.X = x + (z - (z & 1)) / 2;
            vec.Y = z;

            return vec;
        }

        public Vector2 CubedToOffset(Vector3 cubedCoordinates)
        {
            return CubedToOffset((int)cubedCoordinates.X, (int)cubedCoordinates.Y, (int)cubedCoordinates.Z);
        }
        #endregion

        #region Enumerable
        public IEnumerator<T> GetEnumerator()
        {
            foreach (T t in grid)
            {
                yield return t;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion

        private class PathLocation : IComparable
        {
            public int cost { get; private set; }
            public T HexGridObject { get; private set; }

            public PathLocation(T obj, int cost)
            {
                this.HexGridObject = obj;
                this.cost = cost;
            }

            public int CompareTo(object obj)
            {
                return ((PathLocation)obj).cost - this.cost;
            }

            public override bool Equals(object obj)
            {
                if (obj == null || !(obj is PathLocation))
                    return false;

                if (obj == this)
                    return true;

                return this.HexGridObject.HexGridLocation == ((PathLocation)obj).HexGridObject.HexGridLocation;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }

    public enum HexDirection
    {
        E, NE, NW, W, SW, SE
    }
}