using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace NKKD
{

	//各パーツの描画位置決定および描画
	[UpdateAfter(typeof(PreLateUpdate.ParticleSystemBeginUpdateAll))]
	public class CharaDrawSystem : ComponentSystem
	{

		struct Group
		{
			public readonly int Length;
			public EntityArray entities;
			[ReadOnly] public ComponentDataArray<Position> position;
			[ReadOnly] public ComponentDataArray<CharaLook> look;
			[ReadOnly] public ComponentDataArray<CharaMotion> motion;
			[ReadOnly] public ComponentDataArray<CharaTag> tag;
			[ReadOnly] public SharedComponentDataArray<AniBasePos> aniBasePos;
			[ReadOnly] public SharedComponentDataArray<AniScriptSheet> aniScriptSheet;
			[ReadOnly] public SharedComponentDataArray<MeshMatList> ariMeshMat;
		}

		[Inject] Group group;

		//カリング
		[BurstCompileAttribute]
		struct JobCulling : IJobParallelFor
		{
			public NativeArray<int> isInCamera;
			[ReadOnly] public float cameraXMin;
			[ReadOnly] public float cameraXMax;
			[ReadOnly] public float cameraYMin;
			[ReadOnly] public float cameraYMax;
			[ReadOnly] public NativeArray<Position> position;
			[ReadOnly] public int entitiesLength;
			public void Execute(int i)
			{

				//isInCamera[i] = 1;
				if (cameraXMax < position[i].Value.x)
				{
					isInCamera[i] = 0;
				}
				else if (cameraXMin > position[i].Value.x)
				{
					isInCamera[i] = 0;
				}
				else if (cameraYMax < position[i].Value.y)
				{
					isInCamera[i] = 0;
				}
				else if (cameraYMin > position[i].Value.y)
				{
					isInCamera[i] = 0;
				}
				else
				{
					isInCamera[i] = 1;
				}
			}
		}

		//胸手足位置
		[BurstCompileAttribute]
		struct JobBody : IJob
		{
			public NativeArray<Matrix4x4> thoraxMatrix;
			public NativeArray<Matrix4x4> gasterMatrix;
			public NativeArray<Matrix4x4> leftArmMatrix;
			public NativeArray<Matrix4x4> rightArmMatrix;
			public NativeArray<Matrix4x4> leftLegMatrix;
			public NativeArray<Matrix4x4> rightLegMatrix;
			public int matrixLength;
			[ReadOnly] public NativeArray<int> isInCamera;
			[ReadOnly] public NativeArray<Position> position;
			[ReadOnly] public NativeArray<CharaLook> look;
			[ReadOnly] public NativeArray<CharaMotion> motion;
			[ReadOnly] public NativeArray<Quaternion> lrQuaternion;
			[ReadOnly] public NativeArray<AniFrame> frame;
			[ReadOnly] public AniBasePos aniBasePos;
			[ReadOnly] public int entitiesLength;
			[ReadOnly] public Vector3 one;

			public void Execute()
			{
				for (int i = 0; i < entitiesLength; i++)
				{
					// if (isInCamera[i] == 0)continue;

					bool isBack = (look[i].isBack != 0);
					bool isLeft = (look[i].isLeft != 0);

					float thoraxDepth = position[i].Value.z;
					float gasterDepth = position[i].Value.z;
					float leftArmDepth = position[i].Value.z;
					float rightArmDepth = position[i].Value.z;
					float leftLegDepth = position[i].Value.z;
					float rightLegDepth = position[i].Value.z;

					if (isBack)
					{
						thoraxDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.Thorax);
						gasterDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.Gaster);
						leftArmDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.LeftArm);
						rightArmDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.RightArm);
						leftLegDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.LeftLeg);
						rightLegDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.RightLeg);
					}
					else
					{
						thoraxDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.Thorax);
						gasterDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.Gaster);
						leftArmDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.LeftArm);
						rightArmDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.RightArm);
						leftLegDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.LeftLeg);
						rightLegDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.RightLeg);
					}

					float thoraxX;
					float gasterX;
					float lArmX;
					float rArmX;
					float lLegX;
					float rLegX;
					Quaternion q;
					if (isLeft)
					{
						thoraxX = position[i].Value.x - aniBasePos.THORAXBASE.x - frame[i].thorax.x;
						gasterX = position[i].Value.x - aniBasePos.GASTERBASE.x - frame[i].gaster.x;
						if (isBack)
						{
							lArmX = position[i].Value.x - aniBasePos.RARMBASE.x - frame[i].rightArm.x;
							rArmX = position[i].Value.x - aniBasePos.LARMBASE.x - frame[i].leftArm.x;
							lLegX = position[i].Value.x - aniBasePos.RLEGBASE.x - frame[i].rightLeg.x;
							rLegX = position[i].Value.x - aniBasePos.LLEGBASE.x - frame[i].leftLeg.x;
						}
						else
						{
							lArmX = position[i].Value.x - aniBasePos.LARMBASE.x - frame[i].leftArm.x;
							rArmX = position[i].Value.x - aniBasePos.RARMBASE.x - frame[i].rightArm.x;
							lLegX = position[i].Value.x - aniBasePos.LLEGBASE.x - frame[i].leftLeg.x;
							rLegX = position[i].Value.x - aniBasePos.RLEGBASE.x - frame[i].rightLeg.x;
						}

						q = lrQuaternion[0];
					}
					else
					{
						thoraxX = position[i].Value.x + aniBasePos.THORAXBASE.x + frame[i].thorax.x;
						gasterX = position[i].Value.x + aniBasePos.GASTERBASE.x + frame[i].gaster.x;
						if (isBack)
						{
							lArmX = position[i].Value.x + aniBasePos.RARMBASE.x + frame[i].rightArm.x;
							rArmX = position[i].Value.x + aniBasePos.LARMBASE.x + frame[i].leftArm.x;
							lLegX = position[i].Value.x + aniBasePos.RLEGBASE.x + frame[i].rightLeg.x;
							rLegX = position[i].Value.x + aniBasePos.LLEGBASE.x + frame[i].leftLeg.x;
						}
						else
						{
							lArmX = position[i].Value.x + aniBasePos.LARMBASE.x + frame[i].leftArm.x;
							rArmX = position[i].Value.x + aniBasePos.RARMBASE.x + frame[i].rightArm.x;
							lLegX = position[i].Value.x + aniBasePos.LLEGBASE.x + frame[i].leftLeg.x;
							rLegX = position[i].Value.x + aniBasePos.RLEGBASE.x + frame[i].rightLeg.x;
						}

						q = lrQuaternion[1];
					}

					float lArmY;
					float rArmY;
					float lLegY;
					float rLegY;
					if (isBack)
					{
						lArmY = position[i].Value.y + aniBasePos.RARMBASE.y + frame[i].rightArm.y;
						rArmY = position[i].Value.y + aniBasePos.LARMBASE.y + frame[i].leftArm.y;
						lLegY = position[i].Value.y + aniBasePos.RLEGBASE.y + frame[i].rightLeg.y;
						rLegY = position[i].Value.y + aniBasePos.LLEGBASE.y + frame[i].leftLeg.y;
					}
					else
					{
						lArmY = position[i].Value.y + aniBasePos.LARMBASE.y + frame[i].leftArm.y;
						rArmY = position[i].Value.y + aniBasePos.RARMBASE.y + frame[i].rightArm.y;
						lLegY = position[i].Value.y + aniBasePos.LLEGBASE.y + frame[i].leftLeg.y;
						rLegY = position[i].Value.y + aniBasePos.RLEGBASE.y + frame[i].rightLeg.y;
					}

					thoraxMatrix[matrixLength] = Matrix4x4.TRS(new Vector3(thoraxX, position[i].Value.y + aniBasePos.THORAXBASE.y + frame[i].thorax.y,
							thoraxDepth),
						q, one);
					gasterMatrix[matrixLength] = Matrix4x4.TRS(new Vector3(gasterX, position[i].Value.y + aniBasePos.GASTERBASE.y + frame[i].gaster.y,
							gasterDepth),
						q, one);
					leftArmMatrix[matrixLength] = Matrix4x4.TRS(new Vector3(lArmX, lArmY, leftArmDepth),
						q, one);
					rightArmMatrix[matrixLength] = Matrix4x4.TRS(new Vector3(rArmX, rArmY, rightArmDepth),
						q, one);
					leftLegMatrix[matrixLength] = Matrix4x4.TRS(new Vector3(lLegX, lLegY, leftLegDepth),
						q, one);
					rightLegMatrix[matrixLength] = Matrix4x4.TRS(new Vector3(rLegX, rLegY, rightLegDepth),
						q, one);
					matrixLength++;
				}
			}
		}

		//触角頭位置
		[BurstCompileAttribute]
		struct JobAntHead : IJob
		{
			public NativeArray<Matrix4x4> antMatrix;
			public NativeArray<Matrix4x4> antMatrix2;
			public NativeArray<Matrix4x4> headMatrix;
			public NativeArray<Matrix4x4> headMatrix2;
			public NativeArray<int> matrixLength;
			[ReadOnly] public NativeArray<int> isInCamera;
			[ReadOnly] public NativeArray<Position> position;
			[ReadOnly] public NativeArray<CharaLook> look;
			[ReadOnly] public NativeArray<CharaMotion> motion;
			[ReadOnly] public NativeArray<Quaternion> lrQuaternion;
			[ReadOnly] public NativeArray<AniFrame> frame;
			[ReadOnly] public AniBasePos aniBasePos;
			[ReadOnly] public int entitiesLength;
			[ReadOnly] public Vector3 one;
			public void Execute()
			{
				int matIndex = 0;
				int matIndex2 = 0;
				for (int i = 0; i < entitiesLength; i++)
				{

					// if (isInCamera[i] == 0)continue;

					bool isBack = (look[i].isBack != 0);
					bool isLeft = (look[i].isLeft != 0);

					float antDepth = position[i].Value.z;
					float headDepth = position[i].Value.z;
					if (isBack)
					{
						antDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.Ant);
						headDepth += aniBasePos.BACKDEPTH.GetData((int)EnumPartsType.Head);
					}
					else
					{
						antDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.Ant);
						headDepth += aniBasePos.FRONTDEPTH.GetData((int)EnumPartsType.Head);
					}

					float antX;
					float headX;
					Quaternion q;
					if (isLeft)
					{
						antX = position[i].Value.x - aniBasePos.ANTBASE.x - frame[i].ant.x;
						headX = position[i].Value.x - aniBasePos.HEADBASE.x - frame[i].head.x;
						q = lrQuaternion[0];
					}
					else
					{
						antX = position[i].Value.x + aniBasePos.ANTBASE.x + frame[i].ant.x;
						headX = position[i].Value.x + aniBasePos.HEADBASE.x + frame[i].head.x;
						q = lrQuaternion[1];
					}

					Matrix4x4 tmpAntMatrix = Matrix4x4.TRS(new Vector3(antX,
							position[i].Value.y + aniBasePos.ANTBASE.y + frame[i].ant.y,
							antDepth),
						q, one);
					Matrix4x4 tmpHeadMatrix = Matrix4x4.TRS(new Vector3(headX,
							position[i].Value.y + aniBasePos.HEADBASE.y + frame[i].head.y,
							headDepth),
						q, one);

					if (isBack)
					{
						antMatrix2[matIndex2] = tmpAntMatrix;
						headMatrix2[matIndex2] = tmpHeadMatrix;
						matIndex2++;
					}
					else
					{
						antMatrix[matIndex] = tmpAntMatrix;
						headMatrix[matIndex] = tmpHeadMatrix;
						matIndex++;
					}
				}
				matrixLength[0] = matIndex;
				matrixLength[1] = matIndex2;
			}
		}

		protected override void OnUpdate()
		{

			NativeArray<int> isInCamera = new NativeArray<int>(group.Length, Allocator.TempJob);
			NativeArray<Quaternion> lrQuaternion = new NativeArray<Quaternion>(2, Allocator.TempJob);
			lrQuaternion[0] = Quaternion.Euler(new Vector3(-90, 0, 180));
			lrQuaternion[1] = Quaternion.Euler(new Vector3(-90, 0, 0));

			//var entityManager = World.Active.GetOrCreateManager<EntityManager>();

			var position = new NativeArray<Position>(group.Length, Allocator.TempJob);
			group.position.CopyTo(position);
			var look = new NativeArray<CharaLook>(group.Length, Allocator.TempJob);
			group.look.CopyTo(look);
			var motion = new NativeArray<CharaMotion>(group.Length, Allocator.TempJob);
			group.motion.CopyTo(motion);
			var frame = new NativeArray<AniFrame>(group.Length, Allocator.TempJob);
			for (int i = 0; i < group.Length; i++)
			{
				frame[i] = Shared.aniScriptSheet.scripts[(int)motion[i].motionType].frames[motion[i].count >> 2];
			}

			float cameraW = Cache.pixelPerfectCamera.refResolutionX >> 1;
			float cameraH = (cameraW * 0.75f);
			var jobCulling = new JobCulling()
			{
				isInCamera = isInCamera,
					position = position,
					cameraXMax = Camera.main.transform.position.x + cameraW,
					cameraXMin = Camera.main.transform.position.x - cameraW,
					cameraYMax = Camera.main.transform.position.y + cameraH,
					cameraYMin = Camera.main.transform.position.y - cameraH,
					entitiesLength = group.entities.Length,
			};

			var jobHandleCulling = jobCulling.Schedule(group.Length, 64);
			jobHandleCulling.Complete();

			var jobBody = new JobBody()
			{
				thoraxMatrix = new NativeArray<Matrix4x4>(group.Length, Allocator.TempJob),
					gasterMatrix = new NativeArray<Matrix4x4>(group.Length, Allocator.TempJob),
					leftArmMatrix = new NativeArray<Matrix4x4>(group.Length, Allocator.TempJob),
					rightArmMatrix = new NativeArray<Matrix4x4>(group.Length, Allocator.TempJob),
					leftLegMatrix = new NativeArray<Matrix4x4>(group.Length, Allocator.TempJob),
					rightLegMatrix = new NativeArray<Matrix4x4>(group.Length, Allocator.TempJob),
					isInCamera = isInCamera,
					matrixLength = 0,
					position = position,
					look = look,
					motion = motion,
					aniBasePos = Shared.aniBasePos,
					frame = frame,
					lrQuaternion = lrQuaternion,
					entitiesLength = group.entities.Length,
					one = Vector3.one,
			};
			//var jobHandle = job.Schedule(group.Length, 50);
			var jobBodyHandle = jobBody.Schedule();

			const int HEADNUM = 2;
			var jobAntHead = new JobAntHead()
			{
				antMatrix = new NativeArray<Matrix4x4>(group.Length, Allocator.TempJob),
					antMatrix2 = new NativeArray<Matrix4x4>(group.Length, Allocator.TempJob),
					headMatrix = new NativeArray<Matrix4x4>(group.Length, Allocator.TempJob),
					headMatrix2 = new NativeArray<Matrix4x4>(group.Length, Allocator.TempJob),
					matrixLength = new NativeArray<int>(HEADNUM, Allocator.TempJob),
					isInCamera = isInCamera,
					position = position,
					look = look,
					motion = motion,
					aniBasePos = Shared.aniBasePos,
					frame = frame,
					lrQuaternion = lrQuaternion,
					entitiesLength = group.entities.Length,
					one = Vector3.one,
			};
			var jobAntHeadHandle = jobAntHead.Schedule();

			jobBodyHandle.Complete();
			jobAntHeadHandle.Complete();

			//DrawMeshInstancedだとZソートがかからない（最初に描画されたやつに引っ張られてる？）
			//体
			for (int i = 0; i < jobBody.thoraxMatrix.Length; i++)
			{
				Graphics.DrawMesh(group.ariMeshMat[0].meshs[0], jobBody.thoraxMatrix[i],
					group.ariMeshMat[0].materials[0], 0);
				Graphics.DrawMesh(group.ariMeshMat[0].meshs[1], jobBody.leftArmMatrix[i],
					group.ariMeshMat[0].materials[1], 0);
				Graphics.DrawMesh(group.ariMeshMat[0].meshs[1], jobBody.rightArmMatrix[i],
					group.ariMeshMat[0].materials[1], 0);
				Graphics.DrawMesh(group.ariMeshMat[0].meshs[2], jobBody.leftLegMatrix[i],
					group.ariMeshMat[0].materials[2], 0);
				Graphics.DrawMesh(group.ariMeshMat[0].meshs[2], jobBody.rightLegMatrix[i],
					group.ariMeshMat[0].materials[2], 0);
				//Graphics.DrawMesh(group.ariMeshMat[0].meshs[3], jobBody.gasterMatrix[i],
				//	group.ariMeshMat[0].materials[3], 0);
			}

			//頭
			for (int i = 0; i < jobAntHead.matrixLength[0]; i++)
			{
				//Graphics.DrawMesh(group.ariMeshMat[0].meshs[4], jobAntHead.antMatrix[i],
				//	group.ariMeshMat[0].materials[4], 0);
				Graphics.DrawMesh(group.ariMeshMat[0].meshs[5], jobAntHead.headMatrix[i],
					group.ariMeshMat[0].materials[5], 0);
			}
			for (int i = 0; i < jobAntHead.matrixLength[1]; i++)
			{
				//Graphics.DrawMesh(group.ariMeshMat[0].meshs[6], jobAntHead.antMatrix2[i],
				//	group.ariMeshMat[0].materials[6], 0);
				Graphics.DrawMesh(group.ariMeshMat[0].meshs[7], jobAntHead.headMatrix2[i],
					group.ariMeshMat[0].materials[7], 0);
			}

			//NativeArrayの開放

			jobBody.thoraxMatrix.Dispose();
			jobBody.gasterMatrix.Dispose();
			jobBody.leftArmMatrix.Dispose();
			jobBody.rightArmMatrix.Dispose();
			jobBody.leftLegMatrix.Dispose();
			jobBody.rightLegMatrix.Dispose();

			jobAntHead.antMatrix.Dispose();
			jobAntHead.antMatrix2.Dispose();
			jobAntHead.headMatrix.Dispose();
			jobAntHead.headMatrix2.Dispose();
			jobAntHead.matrixLength.Dispose();

			lrQuaternion.Dispose();
			position.Dispose();
			look.Dispose();
			motion.Dispose();
			frame.Dispose();
			isInCamera.Dispose();
		}
	}
}

////胸
//Graphics.DrawMeshInstanced(group.ariMeshMat[0].meshs[0], 0,
//	group.ariMeshMat[0].materials[0], job.thoraxMatrix.ToArray(), job.thoraxMatrix.Length);
////腕
//Graphics.DrawMeshInstanced(group.ariMeshMat[0].meshs[1], 0,
//	group.ariMeshMat[0].materials[1], job.leftArmMatrix.ToArray(), job.leftArmMatrix.Length);
//Graphics.DrawMeshInstanced(group.ariMeshMat[0].meshs[1], 0,
//	group.ariMeshMat[0].materials[1], job.rightArmMatrix.ToArray(), job.rightArmMatrix.Length);
////足
//Graphics.DrawMeshInstanced(group.ariMeshMat[0].meshs[2], 0,
//	group.ariMeshMat[0].materials[2], job.leftLegMatrix.ToArray(), job.leftLegMatrix.Length);
//Graphics.DrawMeshInstanced(group.ariMeshMat[0].meshs[2], 0,
//	group.ariMeshMat[0].materials[2], job.rightLegMatrix.ToArray(), job.rightLegMatrix.Length);

////腹
//Graphics.DrawMeshInstanced(group.ariMeshMat[0].meshs[3], 0,
//	group.ariMeshMat[0].materials[3], jobGaster.gasterMatrix.ToArray(), jobGaster.matrixLength[0]);
////腹
//Graphics.DrawMeshInstanced(group.ariMeshMat[0].meshs[4], 0,
//	group.ariMeshMat[0].materials[4], jobGaster.gasterMatrix2.ToArray(), jobGaster.matrixLength[1]);

////アンテナと顔
//Graphics.DrawMeshInstanced(group.ariMeshMat[0].meshs[6], 0,
//	group.ariMeshMat[0].materials[6], jobAntHead.antMatrix.ToArray(), jobAntHead.matrixLength[0]);
//Graphics.DrawMeshInstanced(group.ariMeshMat[0].meshs[7], 0,
//	group.ariMeshMat[0].materials[7], jobAntHead.headMatrix.ToArray(), jobAntHead.matrixLength[0]);

////アンテナと顔
//Graphics.DrawMeshInstanced(group.ariMeshMat[0].meshs[8], 0,
//	group.ariMeshMat[0].materials[8], jobAntHead.antMatrix2.ToArray(), jobAntHead.matrixLength[1]);
//Graphics.DrawMeshInstanced(group.ariMeshMat[0].meshs[9], 0,
//	group.ariMeshMat[0].materials[9], jobAntHead.headMatrix2.ToArray(), jobAntHead.matrixLength[1]);