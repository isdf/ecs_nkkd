using System;
using System.Collections;
using UnityEngine;

namespace NKKD.EDIT
{
	[Serializable] //シリアライズするため必要
	public struct PartsPos
	{
		public Vector2Int pos;
		public int curveX; //0等速1sin2cos
		public int curveY; //0等速1sin2cos
	}

	//タイムライン：位置
	[Serializable]
	public struct MotionPos
	{
		public PartsPos head;
		public PartsPos body;
		public PartsPos leftArm;
		public PartsPos rightArm;
		public PartsPos leftHand;
		public PartsPos rightHand;
		public PartsPos leftLeg;
		public PartsPos rightLeg;
		public PartsPos leftFoot;
		public PartsPos rightFoot;
		public PartsPos ant;
		public PartsPos core;

		//カーブの計算
		public static float IntermediateCurve(float frame, enCurve curve, float stPos, float edPos)
		{
			float edPer = 0;
			//速度のカーブなので位置自体はサインコサインが逆になる
			switch (curve)
			{
				case enCurve.Normal:
					//アンテナを浮かせたい時はこっちで
					// if (isAnt)
					// {
					// 	edPer = Mathf.Pow(frame, 6);
					// }
					// else
					// {
					// 	edPer = frame;
					// }
					edPer = frame;
					break;
				case enCurve.SinCurve:
					edPer = 1 - Mathf.Cos(frame * Mathf.PI / 2);
					break;
				case enCurve.CosCurve:
					edPer = Mathf.Sin(frame * Mathf.PI / 2);
					break;
				case enCurve.Sin180Curve:
					edPer = (1 - Mathf.Cos(frame * Mathf.PI)) / 2;
					break;
				case enCurve.Exp2Curve:
					edPer = Mathf.Pow(frame, 2);
					break;
				case enCurve.Log2Curve:
					edPer = 1 - Mathf.Pow(1 - frame, 2);
					break;
				case enCurve.Exp3Curve:
					edPer = Mathf.Pow(frame, 3);
					break;
				case enCurve.Log3Curve:
					edPer = 1 - Mathf.Pow(1 - frame, 3);
					break;
				case enCurve.Exp4Curve:
					edPer = Mathf.Pow(frame, 4);
					break;
				case enCurve.Log4Curve:
					edPer = 1 - Mathf.Pow(1 - frame, 4);
					break;
				case enCurve.Exp6Curve:
					edPer = Mathf.Pow(frame, 6);
					break;
				case enCurve.Log6Curve:
					edPer = 1 - Mathf.Pow(1 - frame, 6);
					break;

			}
			float stPer = (1 - edPer);
			return (stPos * stPer) + (edPos * edPer);
		}

		//中割り
		public static MotionPosState MakeIntermediate2(MotionPos stPos, MotionPos edPos,
			float start, float span, float pos)
		{
			MotionPosState res = new MotionPosState();
			float frame = (((pos + 1) - start) / span); //0~1

			foreach (enEditPartsType item in Enum.GetValues(typeof(enEditPartsType)))
			{
				float posx = IntermediateCurve(frame, edPos.GetCurveX(item), stPos.GetPos(item).x, edPos.GetPos(item).x);
				float posy = IntermediateCurve(frame, edPos.GetCurveY(item), stPos.GetPos(item).y, edPos.GetPos(item).y);

				if (item != enEditPartsType.Core)
				{
					//ArmLegAnt以外
					res.SetPos(item, new Vector2Int(
						(int)Math.Round(posx),
						(int)Math.Round(posy)));
				}
			}

			float coreX = IntermediateCurve(frame, edPos.GetCurveX(enEditPartsType.Core), stPos.GetPos(enEditPartsType.Core).x, edPos.GetPos(enEditPartsType.Core).x);
			float coreY = IntermediateCurve(frame, edPos.GetCurveY(enEditPartsType.Core), stPos.GetPos(enEditPartsType.Core).y, edPos.GetPos(enEditPartsType.Core).y);

			var armX = Mathf.Sin(coreY * Mathf.Deg2Rad) * BasePosition.CORE_SIZE;
			var legX = Mathf.Sin(coreX * Mathf.Deg2Rad) * BasePosition.CORE_SIZE;

			res.SetPosJoint((int)Math.Round(armX), (int)Math.Round(legX));

			return res;

		}

		public void SetPos(enPartsType partsType, Vector2Int pos)
		{
			switch (partsType)
			{
				case enPartsType.Body:
					body.pos = pos;
					break;
				case enPartsType.Head:
					head.pos = pos;
					break;
				case enPartsType.LeftArm:
					leftArm.pos = pos;
					break;
				case enPartsType.RightArm:
					rightArm.pos = pos;
					break;
				case enPartsType.LeftHand:
					leftHand.pos = pos;
					break;
				case enPartsType.RightHand:
					rightHand.pos = pos;
					break;
				case enPartsType.LeftLeg:
					leftLeg.pos = pos;
					break;
				case enPartsType.RightLeg:
					rightLeg.pos = pos;
					break;
				case enPartsType.LeftFoot:
					leftFoot.pos = pos;
					break;
				case enPartsType.RightFoot:
					rightFoot.pos = pos;
					break;
				case enPartsType.Ant:
					ant.pos = pos;
					break;
			}
		}
		public Vector2Int GetPos(enEditPartsType partsType)
		{
			Vector2Int res = Vector2Int.zero;
			switch (partsType)
			{
				case enEditPartsType.Body:
					res = body.pos;
					break;
				case enEditPartsType.Head:
					res = head.pos;
					break;
					// case enEditPartsType.LeftArm:
					// 	res = leftArm.pos;
					// 	break;
					// case enPartsType.RightArm:
					// 	res = rightArm.pos;
					// 	break;
				case enEditPartsType.LeftHand:
					res = leftHand.pos;
					break;
				case enEditPartsType.RightHand:
					res = rightHand.pos;
					break;
					// case enPartsType.LeftLeg:
					// 	res = leftLeg.pos;
					// 	break;
					// case enPartsType.RightLeg:
					// 	res = rightLeg.pos;
					// 	break;
				case enEditPartsType.LeftFoot:
					res = leftFoot.pos;
					break;
				case enEditPartsType.RightFoot:
					res = rightFoot.pos;
					break;
					// case enEditPartsType.Ant:
					// 	res = ant.pos;
					// 	break;
				case enEditPartsType.Core:
					res = core.pos;
					break;
			}
			return res;
		}

		public Vector2Int GetPos(enPartsType partsType)
		{
			Vector2Int res = Vector2Int.zero;
			switch (partsType)
			{
				case enPartsType.Body:
					res = body.pos;
					break;
				case enPartsType.Head:
					res = head.pos;
					break;
				case enPartsType.LeftArm:
					res = leftArm.pos;
					break;
				case enPartsType.RightArm:
					res = rightArm.pos;
					break;
				case enPartsType.LeftHand:
					res = leftHand.pos;
					break;
				case enPartsType.RightHand:
					res = rightHand.pos;
					break;
				case enPartsType.LeftLeg:
					res = leftLeg.pos;
					break;
				case enPartsType.RightLeg:
					res = rightLeg.pos;
					break;
				case enPartsType.LeftFoot:
					res = leftFoot.pos;
					break;
				case enPartsType.RightFoot:
					res = rightFoot.pos;
					break;
				case enPartsType.Ant:
					res = ant.pos;
					break;
					// case enPartsType.Core:
					// 	res = core.pos;
					// 	break;
			}
			return res;
		}

		public void SetCurveX(enEditPartsType partsType, enCurve curveX)
		{
			int intCurveX = (int)curveX;
			switch (partsType)
			{
				case enEditPartsType.Body:
					body.curveX = intCurveX;
					break;
				case enEditPartsType.Head:
					head.curveX = intCurveX;
					break;
					// case enPartsType.LeftArm:
					// 	leftArm.curveX = intCurveX;
					// 	break;
					// case enPartsType.RightArm:
					// 	rightArm.curveX = intCurveX;
					// 	break;
				case enEditPartsType.LeftHand:
					leftHand.curveX = intCurveX;
					break;
				case enEditPartsType.RightHand:
					rightHand.curveX = intCurveX;
					break;
					// case enPartsType.LeftLeg:
					// 	leftLeg.curveX = intCurveX;
					// 	break;
					// case enPartsType.RightLeg:
					// 	rightLeg.curveX = intCurveX;
					// 	break;
				case enEditPartsType.LeftFoot:
					leftFoot.curveX = intCurveX;
					break;
				case enEditPartsType.RightFoot:
					rightFoot.curveX = intCurveX;
					break;
					// case enPartsType.Ant:
					// 	ant.curveX = intCurveX;
					// 	break;
				case enEditPartsType.Core:
					core.curveX = intCurveX;
					break;
			}
		}
		public enCurve GetCurveX(enEditPartsType partsType)
		{
			int res = 0;
			switch (partsType)
			{
				case enEditPartsType.Body:
					res = body.curveX;
					break;
				case enEditPartsType.Head:
					res = head.curveX;
					break;
					// case enPartsType.LeftArm:
					// 	res = leftArm.curveX;
					// 	break;
					// case enPartsType.RightArm:
					// 	res = rightArm.curveX;
					// 	break;
				case enEditPartsType.LeftHand:
					res = leftHand.curveX;
					break;
				case enEditPartsType.RightHand:
					res = rightHand.curveX;
					break;
					// case enPartsType.LeftLeg:
					// 	res = leftLeg.curveX;
					// 	break;
					// case enPartsType.RightLeg:
					// 	res = rightLeg.curveX;
					// 	break;
				case enEditPartsType.LeftFoot:
					res = leftFoot.curveX;
					break;
				case enEditPartsType.RightFoot:
					res = rightFoot.curveX;
					break;
					// case enPartsType.Ant:
					// 	res = ant.curveX;
					// 	break;
				case enEditPartsType.Core:
					res = core.curveX;
					break;
			}
			return (enCurve)res;
		}

		public void SetCurveY(enEditPartsType partsType, enCurve curveY)
		{
			int intCurveY = (int)curveY;
			switch (partsType)
			{
				case enEditPartsType.Body:
					body.curveY = intCurveY;
					break;
				case enEditPartsType.Head:
					head.curveY = intCurveY;
					break;
					// case enPartsType.LeftArm:
					// 	leftArm.curveY = intCurveY;
					// 	break;
					// case enPartsType.RightArm:
					// 	rightArm.curveY = intCurveY;
					// 	break;
				case enEditPartsType.LeftHand:
					leftHand.curveY = intCurveY;
					break;
				case enEditPartsType.RightHand:
					rightHand.curveY = intCurveY;
					break;
					// case enPartsType.LeftLeg:
					// 	leftLeg.curveY = intCurveY;
					// 	break;
					// case enPartsType.RightLeg:
					// 	rightLeg.curveY = intCurveY;
					// 	break;
				case enEditPartsType.LeftFoot:
					leftFoot.curveY = intCurveY;
					break;
				case enEditPartsType.RightFoot:
					rightFoot.curveY = intCurveY;
					break;
					// case enPartsType.Ant:
					// 	ant.curveY = intCurveY;
					// 	break;
				case enEditPartsType.Core:
					core.curveY = intCurveY;
					break;
			}
		}
		public enCurve GetCurveY(enEditPartsType partsType)
		{
			int res = 0;
			switch (partsType)
			{
				case enEditPartsType.Body:
					res = body.curveY;
					break;
				case enEditPartsType.Head:
					res = head.curveY;
					break;
					// case enPartsType.LeftArm:
					// 	res = leftArm.curveY;
					// 	break;
					// case enPartsType.RightArm:
					// 	res = rightArm.curveY;
					// 	break;
				case enEditPartsType.LeftHand:
					res = leftHand.curveY;
					break;
				case enEditPartsType.RightHand:
					res = rightHand.curveY;
					break;
					// case enPartsType.LeftLeg:
					// 	res = leftLeg.curveY;
					// 	break;
					// case enPartsType.RightLeg:
					// 	res = rightLeg.curveY;
					// 	break;
				case enEditPartsType.LeftFoot:
					res = leftFoot.curveY;
					break;
				case enEditPartsType.RightFoot:
					res = rightFoot.curveY;
					break;
					// case enPartsType.Ant:
					// 	res = ant.curveY;
					// 	break;
				case enEditPartsType.Core:
					res = core.curveY;
					break;
			}
			return (enCurve)res;
		}

	}
}