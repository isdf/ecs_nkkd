using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace NKKD
{

	//座標移動
	public class BehaveMoveSystem : JobComponentSystem
	{
		[ComputeJobOptimization]
		struct PositionJob : IJobProcessComponentData<Position, CharaBehave>
		{
			public float deltaTime;

			public void Execute(ref Position position, ref CharaBehave behave)
			{

				if (behave.behaveType == (int)EnumBehave.Wander)
				{
					const float SPD = 20f;
					float2 xy = deltaTime * behave.targetVecNrm * SPD;
					position.Value.x += xy.x;
					position.Value.y += xy.y;
				}
				position.Value.z = -100f + position.Value.y * 0.01f;
			}
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var job = new PositionJob()
			{
				deltaTime = Time.deltaTime
			};
			return job.Schedule(this);
			// return job.Schedule(this, 64, inputDeps);
		}
	}
}