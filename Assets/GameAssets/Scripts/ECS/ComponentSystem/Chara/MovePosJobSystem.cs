﻿using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace NKKD
{

	//座標移動
	public class MovePosJobSystem : JobComponentSystem
	{
		ComponentGroup m_group;

		protected override void OnCreateManager()
		{
			m_group = GetComponentGroup(
				ComponentType.Create<CharaMove>());
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var job = new PositionJob()
			{
				m_charaMove = m_group.GetComponentDataArray<CharaMove>(),
			};
			inputDeps = job.Schedule(inputDeps);
			inputDeps.Complete();
			return inputDeps;
			// return job.Schedule(this, 64, inputDeps);
		}

		[BurstCompileAttribute]
		struct PositionJob : IJob
		{
			public ComponentDataArray<CharaMove> m_charaMove;
			public void Execute()
			{
				for (int i = 0; i < m_charaMove.Length; i++)
				{
					var charaMove = m_charaMove[i];
					charaMove.Move();
					m_charaMove[i] = charaMove;
				}
			}
		}

	}
}