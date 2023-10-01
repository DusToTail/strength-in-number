using System;
using UnityEngine;

namespace StrengthInNumber
{
    [CreateAssetMenu(fileName = "Grid2DSettings", menuName = "Custom/Grid2DSettings")]
    [Serializable]
    public class Grid2DSettingsSO : ScriptableObject
    {
        public int xCount;
        public int yCount;
    }
}
