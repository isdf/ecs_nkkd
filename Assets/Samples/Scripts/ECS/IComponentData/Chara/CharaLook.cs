using Unity.Entities;

/// <summary>
/// 向きと表情
/// </summary>
public struct CharaLook : IComponentData
{
    public int isLeft;
    public int isBack;
    public int faceNo;
}