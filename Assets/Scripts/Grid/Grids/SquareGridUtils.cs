using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;

namespace StrengthInNumber.Grid
{
    public static class SquareGridUtils
    {
        public enum Faces
        {
            None,
            Front,
            Back,
            Left,
            Right
        }

        public static Vector2Int ToVector2Int(Faces face)
        {
            switch(face)
            {
                case Faces.Front:
                    {
                        return Vector2Int.up;
                    }
                case Faces.Back:
                    {
                        return Vector2Int.down;
                    }
                case Faces.Left:
                    {
                        return Vector2Int.left;
                    }
                case Faces.Right:
                    {
                        return Vector2Int.right;
                    }
                default:
                {
                    return default;
                }
            }
        }

        public static int2 ToInt2(Faces face)
        {
            switch (face)
            {
                case Faces.Front:
                    {
                        return new int2(0, 1);
                    }
                case Faces.Back:
                    {
                        return new int2(0, -1);
                    }
                case Faces.Left:
                    {
                        return new int2(-1, 0);
                    }
                case Faces.Right:
                    {
                        return new int2(1, 0);
                    }
                default:
                    {
                        return default;
                    }
            }
        }

        public static bool IsOutOfBound(int x, int y, int width, int height)
        {
            if (x < 0 || x > width - 1 ||
                y < 0 || y > height - 1)
            {
                return true;
            }
            return false;
        }
    }
}
