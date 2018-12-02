using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct AniFrame : ISharedComponentData
{
    public Vector2Int ant;
    public Vector2Int head;
    public Vector2Int thorax;
    public Vector2Int gaster;
    public Vector2Int leftArm;
    public Vector2Int rightArm;
    public Vector2Int leftLeg;
    public Vector2Int rightLeg;
    public int angle;
    public int face;
}