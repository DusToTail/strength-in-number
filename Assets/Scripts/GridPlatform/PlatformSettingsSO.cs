using UnityEngine;

namespace StrengthInNumber.GridPlatform
{
    [CreateAssetMenu(fileName = "PlatformSettings", menuName = "Custom/PlatformSettings")]
    public class PlatformSettingsSO : ScriptableObject
    {
        public Grid2DSettingsSO grid;
        public float cellWidth;
        public float cellHeight;
        public PivotPoint cellPivot;
    }
}
