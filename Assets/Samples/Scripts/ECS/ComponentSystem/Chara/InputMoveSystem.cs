using System;
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
	public class InputMoveSystem : ComponentSystem
	{
		ComponentGroup group;

		protected override void OnCreateManager()
		{
			group = GetComponentGroup(
				ComponentType.ReadOnly<CharaMove>(),
				ComponentType.ReadOnly<CharaDash>(),
				ComponentType.ReadOnly<CharaMotion>(),
				ComponentType.ReadOnly<PadInput>());
		}
		ComponentDataArray<CharaMove> charaMoves;
		ComponentDataArray<CharaDash> charaDashs;
		ComponentDataArray<CharaMotion> charaMotions;
		ComponentDataArray<PadInput> padInputs;

		protected override void OnUpdate()
		{
			charaMoves = group.GetComponentDataArray<CharaMove>();
			charaDashs = group.GetComponentDataArray<CharaDash>();
			charaMotions = group.GetComponentDataArray<CharaMotion>();
			padInputs = group.GetComponentDataArray<PadInput>();

			for (int i = 0; i < charaMotions.Length; i++)
			{
				//モーションごとの入力
				switch (charaMotions[i].motionType)
				{
					case EnumMotion.Idle:
						Friction(i);
						break;
					case EnumMotion.Walk:
						break;
					case EnumMotion.Dash:
						break;
					case EnumMotion.Slip:
						Friction(i);
						break;
					case EnumMotion.Jump:
						break;
					case EnumMotion.Fall:
						break;
					case EnumMotion.Land:
						Stop(i);
						break;
					case EnumMotion.Damage:
						break;
					case EnumMotion.Fly:
						break;
					case EnumMotion.Down:
						Friction(i);
						break;
					case EnumMotion.Dead:
						Stop(i);
						break;
					case EnumMotion.Action:
						break;
					default:
						Debug.Assert(false);
						break;
				}
			}
		}

		/// <summary>
		/// 摩擦
		/// </summary>
		/// <param name="i"></param>
		void Friction(int i)
		{
			var charaMove = charaMoves[i];
			charaMove.Friction();
			charaMoves[i] = charaMove;
		}

		/// <summary>
		/// 停止
		/// </summary>
		/// <param name="i"></param>
		void Stop(int i)
		{
			var charaMove = charaMoves[i];
			charaMove.Stop();
			charaMoves[i] = charaMove;
		}
	}

}