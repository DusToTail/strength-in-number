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
        public static readonly Dictionary<Faces, Vector2Int> Directions = new Dictionary<Faces, Vector2Int>()
        {
            {Faces.None, Vector2Int.zero },
            {Faces.Front, Vector2Int.up },
            {Faces.Back, Vector2Int.down },
            {Faces.Left, Vector2Int.left },
            {Faces.Right, Vector2Int.right }
        };

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
