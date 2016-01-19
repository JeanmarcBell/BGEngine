using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine
{
    public class Quadtree<T>
    {
        private class QuadTreeValue
        {
            public T UserData { get; set; }
            public RectangleF Bounds { get; set; }

            public QuadTreeValue(T userData, RectangleF bounds)
            {
                UserData = userData;
                Bounds = bounds;
            }
        }

        private static readonly int MAX_DEPTH = 10;
        private static readonly int MAX_OBJECTS = 4;
        private int depth;
        private List<QuadTreeValue> contents;

        public Quadtree<T>[] SubNodes { get; private set; }
        public RectangleF Bounds { get; private set; }
        public List<T> Contents
        {
            get
            {
                List<T> contentValues = new List<T>();
                foreach (QuadTreeValue value in contents)
                    contentValues.Add(value.UserData);
                return contentValues;
            }
        }

        public Quadtree(RectangleF bounds, int depth = 0)
        {
            this.Bounds = bounds;
            this.depth = depth;
            SubNodes = new Quadtree<T>[4];
            contents = new List<QuadTreeValue>();
        }

        /// <summary>
        /// Inserts an object into the QuadTree
        /// </summary>
        /// <param name="obj"></param>
        public void Insert(T obj, RectangleF bounds)
        {
            if (SubNodes[0] != null)
            {
                int index = GetIndex(bounds);
                if(index != -1)
                {
                    SubNodes[index].Insert(obj, bounds);
                    return;
                }
            }

            contents.Add(new QuadTreeValue(obj, bounds));
            if (contents.Count > MAX_OBJECTS && depth < MAX_DEPTH && SubNodes[0] == null)
            {
                Split();                
            }
        }

        /// <summary>
        /// Clears all elements from the Quadtree
        /// </summary>
        public void Clear()
        {
            contents.Clear();

            for (int i = 0; i < SubNodes.Length; i++)
            {
                if (SubNodes[i] != null)
                {
                    SubNodes[i].Clear();
                    SubNodes[i] = null;
                }
            }
        }

        /// <summary>
        /// Gets a list of all objects that might be near a particular object
        /// </summary>
        /// <param name="obj">The object to check for nearness against</param>
        public List<T> Query(RectangleF bounds)
        {
            List<T> returnList = new List<T>();
            QueryRecursive(bounds, returnList);
            return returnList;
        }

        private void QueryRecursive(RectangleF bounds, List<T> returnList) // Helper function for Query
        {
            int index = GetIndex(bounds);
            if (index != -1 && SubNodes[0] != null)
            {
                SubNodes[index].QueryRecursive(bounds, returnList);
            }
            foreach (QuadTreeValue value in contents)
                returnList.Add(value.UserData);            
        }

        private void Split() // Splits the contents into subnodes
        {
            float subWidth = Bounds.Width / 2f;
            float subHeight = Bounds.Height / 2f;
 
            SubNodes[1] = new Quadtree<T>(new RectangleF(Bounds.Left + subWidth, Bounds.Top, subWidth, subHeight), depth + 1);
            SubNodes[0] = new Quadtree<T>(new RectangleF(Bounds.Left, Bounds.Top, subWidth, subHeight), depth + 1);
            SubNodes[2] = new Quadtree<T>(new RectangleF(Bounds.Left, Bounds.Top + subHeight, subWidth, subHeight), depth + 1);
            SubNodes[3] = new Quadtree<T>(new RectangleF(Bounds.Left + subWidth, Bounds.Top + subHeight, subWidth, subHeight), depth + 1);

            int i = 0;
            while (i < contents.Count)
            {
                int index = GetIndex(contents[i].Bounds);
                if (index != -1)
                {
                    SubNodes[index].Insert(contents[i].UserData, contents[i].Bounds);
                    contents.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        private int GetIndex(RectangleF bounds) // Get the index of the subnode that an object falls within (-1 if it lies on a boundary)
        {
            int index = -1;

            float verticalMidpoint = Bounds.Left + (Bounds.Width / 2);
            float horizontalMidpoint = Bounds.Top + (Bounds.Height / 2);

            bool isTopQuadrant = bounds.Bottom < horizontalMidpoint;
            bool isBottomQuadrant = bounds.Top > horizontalMidpoint;
            bool isLeftQuadrant = bounds.Right < verticalMidpoint;
            bool isRightQuadrant = bounds.Left > verticalMidpoint;

            if (isTopQuadrant)
            {
                if (isLeftQuadrant)
                    index = 0;
                else if (isRightQuadrant)
                    index = 1;
            }
            else if (isBottomQuadrant)
            {
                if (isLeftQuadrant)
                    index = 2;
                else if (isRightQuadrant)
                    index = 3;
            }

            return index;
        }
    }
}