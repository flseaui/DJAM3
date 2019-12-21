using System;
using UnityEngine;

[CreateAssetMenu]
public class Time : ScriptableObject
{
    public float InitialTime;
    
    [NonSerialized]
    public float Value;
}