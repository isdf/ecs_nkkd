using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct AniBasePos : ISharedComponentData
{
    public Vector2Int ANTBASE;
    public Vector2Int HEADBASE;
    public Vector2Int THORAXBASE;
    public Vector2Int GASTERBASE;
    public Vector2Int LARMBASE;
    public Vector2Int RARMBASE;
    public Vector2Int LLEGBASE;
    public Vector2Int RLEGBASE;

    public AniDepth FRONTDEPTH;
    public AniDepth BACKDEPTH;
}