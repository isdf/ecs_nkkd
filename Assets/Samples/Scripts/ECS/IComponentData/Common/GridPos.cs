using Unity.Entities;
using UnityEngine;

//チップの座標
public struct GridPos : IComponentData
{
    public Vector2Int Value;
}