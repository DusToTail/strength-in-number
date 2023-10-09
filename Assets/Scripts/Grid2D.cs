using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace StrengthInNumber
{
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
