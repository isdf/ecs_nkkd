using Unity.Entities;
using Unity.Mathematics;
/// <summary>
/// 行動タイプ
/// </summary>
public struct CharaBehave : IComponentData
{
	public int behaveType; //種類
	public float endTime; //終了予定時間
	public int angle; //目的方向
	public float2 targetVecNrm; //目的方向（正規化）
}