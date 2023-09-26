using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace StrengthInNumber
{
    public struct Grid2DUnmanaged<T> : IDisposable where T : unmanaged
    {
        public int xCount;
        public int yCount;
        public NativeArray<T> array;

        public void Fill(T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }
        public void Fill(T value, int index)
        {
            array[index] = value;
        }
        public void Fill(T value, int2 position)
        {
            array[ToIndex(position)] = value;
        }
        public int ToIndex(int2 position) => ToIndex(position.x, position.y);
        public int ToIndex(int x, int y)
        {
            return y * xCount + x;
        }
        public int2 ToPosition(int index)
        {
            int y = index / xCount;
            int x = index - y * xCount;
            return new int2(x, y);
        }
        public void Dispose()
        {
            array.Dispose();
        }
    }

    public class Grid2DManaged<T> where T : unmanaged
    {
        public NativeArray<T>.ReadOnly ArrayRO { get { return _array.AsReadOnly(); } }
        public Grid2DSettingsSO Settings { get; private set; }

        private NativeArray<T> _array;

        public Grid2DManaged(Grid2DSettingsSO settings)
        {
            Settings = settings ?? new Grid2DSettingsSO()
            {
                xCount = 10,
                yCount = 10,
            };

            int total = settings.xCount * settings.yCount;
            _array = new NativeArray<T>(total, Allocator.Persistent);
        }

        public void Fill(T value)
        {
            for(int i = 0; i < ArrayRO.Length; i++)
            {
                _array[i] = value;
            }
        }
        public void Fill(T value, int index)
        {
            _array[index] = value;
        }
        public void Fill(T value, int2 position)
        {
            _array[ToIndex(position)] = value;
        }
        public int ToIndex(int2 position) => ToIndex(position.x, position.y);
        public int ToIndex(int x, int y)
        {
            return y * Settings.xCount + x;
        }
        public int2 ToPosition(int index)
        {
            int y = index / Settings.xCount;
            int x = index - y * Settings.xCount;
            return new int2(x, y);
        }

        ~Grid2DManaged()
        {
            _array.Dispose();
        }
    }


    [CreateAssetMenu(fileName = "Grid2DSettings", menuName = "Custom/Grid2DSettings")]
    [Serializable]
    public class Grid2DSettingsSO : ScriptableObject
    {
        public int xCount;
        public int yCount;
    }
}
