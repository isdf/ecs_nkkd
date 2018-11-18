using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
//using Unity.Transforms2D;
using Unity.Collections;
//using toinfiniityandbeyond.Rendering2D;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEditor;
using UnityEngine.SceneManagement;
namespace NKKD
{
	public static class CharaEntityFactory
	{

		//アリエンティティ作成
		public static Entity CreateEntity(int i, EntityManager entityManager,
			ref MeshMatList ariMeshMat,
			ref AniScriptSheet aniScriptSheet,
			ref AniBasePos aniBasePos
		)
		{
			var ariArchetype = entityManager.CreateArchetype(ComponentTypes.CharaComponentType);

			var entity = entityManager.CreateEntity(ariArchetype);
			//ComponentDataのセット
			var posL = 0;
			//Define.Instance.GetMapSize() / 2;
			var posH = 0;
			//Define.Instance.GetMapSize() / 2;
			//位置
			entityManager.SetComponentData(entity, new Position
			{
				Value = new float3(UnityEngine.Random.Range(posL, posH), UnityEngine.Random.Range(posL, posH), 0)
			});

			//モーション
			entityManager.SetComponentData(entity, new CharaMotion
			{

			});

			//行動
			entityManager.SetComponentData(entity, new CharaBehave
			{
				behaveType = 0,
					angle = (int)UnityEngine.Random.Range(0, 11),
					endTime = (Time.realtimeSinceStartup + 0.5f + UnityEngine.Random.value)
			});

			//ID
			entityManager.SetComponentData(entity, new CharaId
			{
				familyId = 0,
					myId = i
			});
			//向き
			entityManager.SetComponentData(entity, new CharaLook
			{
				isLeft = 0,
					isBack = 0
			});

			//SharedComponentDataのセット
			entityManager.AddSharedComponentData(entity, ariMeshMat);
			entityManager.AddSharedComponentData(entity, aniScriptSheet);
			entityManager.AddSharedComponentData(entity, aniBasePos);

			return entity;
		}
	}
}