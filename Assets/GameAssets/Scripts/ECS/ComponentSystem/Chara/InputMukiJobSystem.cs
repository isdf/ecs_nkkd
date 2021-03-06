﻿using System;
using System.Collections.ObjectModel;
using HedgehogTeam.EasyTouch;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace NKKD
{
	/// <summary>
	/// 入力による向き変化システム
	/// </summary>
	public class InputMukiSystem : JobComponentSystem
	{
		ComponentGroup m_group;

		protected override void OnCreateManager()
		{
			m_group = GetComponentGroup(
				ComponentType.Create<CharaMuki>(),
				ComponentType.ReadOnly<CharaMotion>(),
				ComponentType.ReadOnly<PadInput>());
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var job = new InputJob()
			{
				m_charaMotions = m_group.GetComponentDataArray<CharaMotion>(),
					m_charaMukis = m_group.GetComponentDataArray<CharaMuki>(),
					m_padInputs = m_group.GetComponentDataArray<PadInput>(),
			};

			inputDeps = job.Schedule(inputDeps);
			inputDeps.Complete();
			return inputDeps;
		}

		[BurstCompileAttribute]
		struct InputJob : IJob
		{
			public ComponentDataArray<CharaMuki> m_charaMukis;
			[ReadOnly]
			public ComponentDataArray<CharaMotion> m_charaMotions;
			[ReadOnly]
			public ComponentDataArray<PadInput> m_padInputs;
			public void Execute()
			{

				for (int i = 0; i < m_charaMotions.Length; i++)
				{
					//モーションごとの入力
					switch (m_charaMotions[i].motionType)
					{
						case EnumMotion.Idle:
							break;
						case EnumMotion.Walk:
							CheckCrossX(i);
							break;
						case EnumMotion.Dash:
							break;
						case EnumMotion.Slip:
							break;
						case EnumMotion.Jump:
							CheckCrossX(i);
							break;
						case EnumMotion.Fall:
							CheckCrossX(i);
							break;
						case EnumMotion.Land:
							break;
						case EnumMotion.Damage:
							break;
						case EnumMotion.Fly:
							break;
						case EnumMotion.Down:
							break;
						case EnumMotion.Dead:
							break;
						case EnumMotion.Action:
							break;
						default:
							Debug.Assert(false);
							break;
					}
				}
			}

			//左右チェック
			bool CheckCrossX(int i)
			{
				if (m_padInputs[i].crossLeft.IsPress()
					|| m_padInputs[i].crossRight.IsPress())
				{
					var charaMuki = m_charaMukis[i];
					charaMuki.muki = m_padInputs[i].crossLeft.IsPress()
						? EnumMuki.Left
						: EnumMuki.Right;
					m_charaMukis[i] = charaMuki;
					return true;
				}

				return false;
			}
		}

	}
}