using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace StrengthInNumber
{
    public class Grid2D<T> where T : unmanaged
    {
        public NativeArray<T>.ReadOnly ArrayRO { get { return _array.AsReadOnly(); } }
        public Grid2DSettingsSO Settings { get; private set; }

        private NativeArray<T> _array;

        public Grid2D(Grid2DSettingsSO settings)
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

        public int ToIndex(int2 position)
        {
            return position.y * Settings.xCount + position.x;
        }
        public int2 ToPosition(int index)
        {
            int y = index / Settings.xCount;
            int x = index - y * Settings.xCount;
            return new int2(x, y);
        }

        ~Grid2D()
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
