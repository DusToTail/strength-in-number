using UnityEngine;
using Unity.Mathematics;

[CreateAssetMenu(fileName = "DefaultMovement", menuName = "Custom/MovementAsset")]
public class TranslationSettingsSO : ScriptableObject
{
    public float3 initialVelocity;
    public float3 initialAcceleration;
    public float maxSpeed;
    public float maxAcceleration;
    public float dampFactor;
}