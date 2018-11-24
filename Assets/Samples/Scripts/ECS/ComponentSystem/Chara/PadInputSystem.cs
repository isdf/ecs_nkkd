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
	/// 入力システム
	/// </summary>
	public class PadInputSystem : ComponentSystem
	{
		private readonly EntityArchetypeQuery query = new EntityArchetypeQuery
		{
			None = Array.Empty<ComponentType>(),
				Any = Array.Empty<ComponentType>(),
				All = new [] { ComponentType.Create<PadInput>() },
		};

		protected override void OnUpdate()
		{
			var chunks = EntityManager.CreateArchetypeChunkArray(query, Allocator.TempJob);
			var padInputType = GetArchetypeChunkComponentType<PadInput>(false);
			for (int chunkIndex = 0, length = chunks.Length; chunkIndex < length; chunkIndex++)
			{
				var chunk = chunks[chunkIndex];
				var padInputs = chunk.GetNativeArray(padInputType);
				for (int i = 0, chunkCount = chunk.Count; i < chunkCount; i++)
				{
					var padInput = padInputs[i];
					ScanInput(ref padInput, i);
					padInputs[i] = padInput;
				}
			}

			chunks.Dispose();
		}

		/// <summary>
		/// ボタン名の定数
		/// </summary>
		/// <value></value>
		ReadOnlyCollection<string> ButtonTypeName =
			Array.AsReadOnly(new string[]
			{
				EnumButtonType.Fire1.ToString(),
					EnumButtonType.Fire2.ToString(),
					EnumButtonType.Fire3.ToString(),
					EnumButtonType.Fire4.ToString(),
					EnumButtonType.Fire5.ToString(),
					EnumButtonType.Fire6.ToString(),
			});

		/// <summary>
		/// ゲームパッドからの入力
		/// </summary>
		/// <param name="_padInput"></param>
		/// <param name="_playerNo"></param>
		void ScanInput(ref PadInput _padInput, int _playerNo)
		{
			string player = "P" + _playerNo.ToString();

			//十字
			var nowAxis = new Vector2(Input.GetAxis(player + "Horizontal"), Input.GetAxis(player + "Vertical"));
			_padInput.SetCross(nowAxis, Time.time);

			//ボタン
			foreach (EnumButtonType item in Enum.GetValues(typeof(EnumButtonType)))
			{
				var buttonName = player + ButtonTypeName[(int)item];
				var isPush = Input.GetButtonDown(buttonName);
				var isPress = Input.GetButton(buttonName);
				var isPop = Input.GetButtonUp(buttonName);

				switch (item)
				{
					case EnumButtonType.Fire1:
						_padInput.buttonA.SetData(isPush, isPress, isPop, Time.time);
						break;
					case EnumButtonType.Fire2:
						_padInput.buttonB.SetData(isPush, isPress, isPop, Time.time);
						break;
				}
			}

		}

	}
}