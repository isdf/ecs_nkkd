using Unity.Entities;
using UnityEngine;
namespace NKKD
{
	/// <summary>
	/// キャラの座標移動量
	/// </summary>
	public struct CharaMove : IComponentData
	{
		public Vector3Int delta;

		// XZ方向減速
		public void Friction()
		{
			//ブレーキ量
			const int BRAKE_DELTA_X = 10;
			const int BRAKE_DELTA_Z = 5;

			if (delta.x > 0)
			{
				delta.x = Mathf.Min(0, delta.x - BRAKE_DELTA_X);
			}
			else if (delta.x < 0)
			{
				delta.x = Mathf.Max(0, delta.x + BRAKE_DELTA_X);
			}

			if (delta.z > 0)
			{
				delta.z = Mathf.Min(0, delta.z - BRAKE_DELTA_Z);
			}
			else if (delta.x < 0)
			{
				delta.z = Mathf.Max(0, delta.z + BRAKE_DELTA_Z);
			}
		}

		//XZ方向停止
		public void Stop()
		{
			delta.x = 0;
			delta.z = 0;
		}

	}
}