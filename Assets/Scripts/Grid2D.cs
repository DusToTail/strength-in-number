using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace StrengthInNumber
{
    public enum Shape
    {
        Square,
        Triangle
    }

    public interface IGridSettings
    {
        int Resolution { get; set; }
        int Width { get; }
        int Height { get; }
        float2 Center { get; set; }
        Shape Shape { get;}
        float CellSize { get; set; }

        int WorldToIndex(float2 position, bool alwaysInGrid);
    }

    public struct SquareGridSettings : IGridSettings
    {
        public int Resolution { get; set; }
        public int Width => Resolution;
        public int Height => Resolution;
        public float2 Center { get; set; }
        public Shape Shape => Shape.Square;
        public float CellSize { get; set; }

        public int WorldToIndex(float2 position, bool alwaysInGrid)
        {
            // For 5x5 (res 5) grid
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            // Å°Å°Å°Å°Å°
            float2 diff = position - Center - new float2(CellSize * Width, CellSize * Height) / 2;
            int x = (int)(diff.x / CellSize);
            int y =(int)(diff.y / CellSize);
            if (alwaysInGrid)
            {
                x = math.clamp(x, 0, Width - 1);
                y = math.clamp(y, 0, Height - 1);
                return y * Width + x;
            }

            if(x < 0 || x > Width - 1 ||
                y < 0 || y > Height - 1)
            {
                return -1;
            }
            return y * Width + x;
        }
    }
    public struct TriangleGridSettings : IGridSettings
    {
        public const float H = 1.732f / 2f;
        public int Resolution { get; set; }
        public int Width => Resolution;
        public int Height => Resolution;
        public float2 Center { get; set; }
        public Shape Shape => Shape.Triangle;
        public float CellSize { get; set; }

        public int WorldToIndex(float2 position, bool alwaysInGrid)
        {
            // For 5x5 (res 5) grid
            // Å£Å•Å£Å•Å£
            // Å•Å£Å•Å£Å•
            // Å£Å•Å£Å•Å£
            // Å•Å£Å•Å£Å•
            // Å£Å•Å£Å•Å£
            float2 diff = position - Center - new float2(CellSize * Width, CellSize * Height) / 2;
            float yFloat = diff.y / (CellSize * H);
            int y = (int)yFloat;
            float xFloat = diff.x / CellSize;
            int x = (int)xFloat;
            float yDecimal = math.frac(yFloat);
            float xDecimal = math.frac(xFloat);
            if (xDecimal != 0.5f) // If x does not lie exactly at the middle of the triangle, needs to evaluate for left and right triangles
            {
                if ((y & 1) == 0) // If even (up/down pattern) row
                {
                    if(xDecimal < 0.5f)
                    {
                        // If C is on the left of AB, subtract index by 1
                        float2 A = float2.zero;
                        float2 B = new float2(CellSize * 0.5f, CellSize * H);
                        float2 C = new float2(xDecimal, yDecimal);
                        float d = TriangleUtils.HalfPlaneCheck(C, A, B);
                        if(d < 0)
                        {
                            x--;
                        }
                    }
                    else
                    {
                        // If C is on the right of AB, add index by 1
                        float2 A = new float2(CellSize * 0.5f, CellSize * H);
                        float2 B = new float2(CellSize, 0f);
                        float2 C = new float2(xDecimal, yDecimal);
                        float d = TriangleUtils.HalfPlaneCheck(C, A, B);
                        if (d > 0)
                        {
                            x++;
                        }
                    }
                }
                else // If odd (down/up pattern) row
                {
                    if (xDecimal < 0.5f)
                    {
                        // If C is on the left of AB, subtract index by 1
                        float2 A = new float2(0f, CellSize * H);
                        float2 B = new float2(CellSize * 0.5f, 0f);
                        float2 C = new float2(xDecimal, yDecimal);
                        float d = TriangleUtils.HalfPlaneCheck(C, A, B);
                        if (d < 0)
                        {
                            x--;
                        }
                    }
                    else
                    {
                        // If C is on the right of AB, add index by 1
                        float2 A = new float2(CellSize * 0.5f, 0f);
                        float2 B = new float2(CellSize, CellSize * H);
                        float2 C = new float2(xDecimal, yDecimal);
                        float d = TriangleUtils.HalfPlaneCheck(C, A, B);
                        if (d > 0)
                        {
                            x++;
                        }
                    }
                }
            }
            
            if (alwaysInGrid)
            {
                x = math.clamp(x, 0, Width - 1);
                y = math.clamp(y, 0, Height - 1);
                return y * Width + x;
            }

            if (x < 0 || x > Width - 1 ||
                y < 0 || y > Height - 1)
            {
                return -1;
            }
            return y * Width + x;
        }
    }

    public struct Grid2DUnmanaged<T> : IDisposable, IEquatable<Grid2DUnmanaged<T>>, INativeDisposable where T : unmanaged
    {
        public int xCount;
        public int yCount;
        public NativeArray<T> array;

        public Grid2DUnmanaged(int xCount, int yCount, NativeArray<T> array)
        {
            this.xCount = xCount;
            this.yCount = yCount;
            this.array = new NativeArray<T>(array, Allocator.Persistent);
        }

        public void Fill(T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }

        public T this [int index]
        {
            get
            {
                return array[index];
            }
            set
            {
                array[index] = value;
            }
        }
        public T this[int x, int y]
        {
            get
            {
                return array[y * xCount + x];
            }
            set
            {
                array[y * xCount + x] = value;
            }
        }

        public JobHandle Dispose(JobHandle inputDeps)
        {
            return array.Dispose(inputDeps);
        }

        public void Dispose()
        {
            array.Dispose();
        }

        public bool Equals(Grid2DUnmanaged<T> other)
        {
            if(xCount == other.yCount &&
               yCount == other.yCount &&
               array.Equals(other.array))
            {
                return true;
            }
            return false;
        }
    }

    public class Grid2DManaged<T> where T : unmanaged
    {
        public NativeArray<T>.ReadOnly ArrayRO { get { return _array.AsReadOnly(); } }
        public int XCount { get; private set; }
        public int YCount { get; private set; }

        private NativeArray<T> _array;

        public Grid2DManaged(int xCount, int yCount, NativeArray<T> array)
        {
            this.XCount = xCount;
            this.YCount = yCount;
            int total = xCount * yCount;
            _array = new NativeArray<T>(array, Allocator.Persistent);
        }

        public void Fill(T value)
        {
            for(int i = 0; i < _array.Length; i++)
            {
                _array[i] = value;
            }
        }

        public T this[int index]
        {
            get
            {
                return _array[index];
            }
            set
            {
                _array[index] = value;
            }
        }
        public T this[int x, int y]
        {
            get
            {
                return _array[y * XCount + x];
            }
            set
            {
                _array[y * XCount + x] = value;
            }
        }

        ~Grid2DManaged()
        {
            _array.Dispose();
        }
    }
}
