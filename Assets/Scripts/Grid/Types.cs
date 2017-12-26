using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Types
{
    /*
     * 2d vector class using integers allows for faster computations and smaller memory consumption
     * Reference for IntVector2 class: https://forum.unity.com/threads/random-level-generator.342587/#post-2215083
     */

	[System.Serializable]
    public class IntVector2
    {
		[SerializeField]
        public int x;
		[SerializeField]
        public int y;

        public static IntVector2 up = new IntVector2(0, 1); //north
        public static IntVector2 right = new IntVector2(1, 0);  //east
        public static IntVector2 down = new IntVector2(0, -1);  //south
        public static IntVector2 left = new IntVector2(-1, 0);  //west

        public static IntVector2 zero = new IntVector2(0, 0);

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int sqrMagnitude
        {
            get { return x * x + y * y; }
        }

        //Casts
        public static implicit operator Vector2(IntVector2 From)
        {
            return new Vector2(From.x, From.y);
        }

        public static implicit operator IntVector2(Vector2 From)
        {
            return new IntVector2((int)From.x, (int)From.y);
        }

        public static implicit operator string(IntVector2 From)
        {
            return "(" + From.x + ", " + From.y + ")";
        }

        //Operators
        public static IntVector2 operator +(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x + b.x, a.y + b.y);
        }

        public static IntVector2 operator +(IntVector2 a, Vector2 b)
        {
            return new IntVector2(a.x + (int)b.x, a.y + (int)b.y);
        }

        public static IntVector2 operator -(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x - b.x, a.y - b.y);
        }

        public static IntVector2 operator -(IntVector2 a, Vector2 b)
        {
            return new IntVector2(a.x - (int)b.x, a.y - (int)b.y);
        }

        public static IntVector2 operator *(IntVector2 a, int b)
        {
            return new IntVector2(a.x * b, a.y * b);
        }

        public static bool operator ==(IntVector2 iv1, IntVector2 iv2)
        {
            return (iv1.x == iv2.x && iv1.y == iv2.y);
        }

        public static bool operator !=(IntVector2 iv1, IntVector2 iv2)
        {
            return (iv1.x != iv2.x || iv1.y != iv2.y);
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            IntVector2 p = obj as IntVector2;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (x == p.x) && (y == p.y);
        }

        public bool Equals(IntVector2 p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (x == p.x) && (y == p.y);
        }

        public override int GetHashCode()
        {
            return x ^ y;
        }
    }
}
