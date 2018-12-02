using UnityEngine;
using System;
using System.Collections;

namespace NKKD.EDIT {
	[Serializable]//シリアライズするため必要
	public struct PartsPos {
		public Vector2Int pos;
		public int curveX;//0等速1sin2cos
		public int curveY;//0等速1sin2cos
	}

	//タイムライン：位置
	[Serializable]
	public struct MotionPos {
		public PartsPos head;
		public PartsPos thorax;
		public PartsPos gaster;
		public PartsPos leftArm;
		public PartsPos rightArm;
		public PartsPos leftLeg;
		public PartsPos rightLeg;
		public PartsPos ant;

		//カーブの計算
		public static float IntermediateCurve(float frame, enCurve curve, float stPos, float edPos, bool isAnt) {
			float edPer = 0;
			//速度のカーブなので位置自体はサインコサインが逆になる
			switch (curve) {
				case enCurve.Normal:
					if (isAnt) {
						edPer = Mathf.Pow(frame, 6);
					}
					else {
						edPer = frame;
					}

					break;
				case enCurve.SinCurve: edPer = 1 - Mathf.Cos(frame * Mathf.PI / 2); break;
				case enCurve.CosCurve: edPer = Mathf.Sin(frame * Mathf.PI / 2); break;
				case enCurve.Sin180Curve: edPer = (1 - Mathf.Cos(frame * Mathf.PI)) / 2; break;
				case enCurve.Exp2Curve: edPer = Mathf.Pow(frame, 2); break;
				case enCurve.Log2Curve: edPer = 1 - Mathf.Pow(1 - frame, 2); break;
				case enCurve.Exp3Curve: edPer = Mathf.Pow(frame, 3); break;
				case enCurve.Log3Curve: edPer = 1 - Mathf.Pow(1 - frame, 3); break;
				case enCurve.Exp4Curve: edPer = Mathf.Pow(frame, 4); break;
				case enCurve.Log4Curve: edPer = 1 - Mathf.Pow(1 - frame, 4); break;
				case enCurve.Exp6Curve: edPer = Mathf.Pow(frame, 6); break;
				case enCurve.Log6Curve: edPer = 1 - Mathf.Pow(1 - frame, 6); break;

			}
			float stPer = (1 - edPer);
			return (stPos * stPer) + (edPos * edPer);
		}

		//中割り
		public static MotionPosState MakeIntermediate2(MotionPos stPos, MotionPos edPos,
			float start, float span, float pos) {
			MotionPosState res = new MotionPosState();
			float frame = (((pos + 1) - start) / span);//0~1

			foreach (enPartsType item in Enum.GetValues(typeof(enPartsType))) {
				float posx = IntermediateCurve(frame, edPos.GetCurveX(item), stPos.GetPos(item).x, edPos.GetPos(item).x, (item == enPartsType.Ant));
				float posy = IntermediateCurve(frame, edPos.GetCurveY(item), stPos.GetPos(item).y, edPos.GetPos(item).y, (item == enPartsType.Ant));
				res.SetPos(item, new Vector2Int(
					(int)Math.Round(posx),
					(int)Math.Round(posy)));
				//if (item == enPartsType.Ant)
				//	Debug.Log(frame.ToString()+" " +  (int)Math.Round(posy));
			}
			return res;

		}

		////中割り
		//public static MotionPosState MakeIntermediate(MotionPosState stPos, MotionPos edPos,
		//	float start, float span, float pos) {
		//	MotionPosState res = new MotionPosState();
		//	float frame = (((pos + 1) - start) / span);//0~1

		//	foreach (enPartsType item in Enum.GetValues(typeof(enPartsType))) {
		//		float posx = IntermediateCurve(frame, edPos.GetCurveX(item), stPos.GetPos(item).x, edPos.GetPos(item).x, (item == enPartsType.Ant));
		//		float posy = IntermediateCurve(frame, edPos.GetCurveY(item), stPos.GetPos(item).y, edPos.GetPos(item).y, (item == enPartsType.Ant));
		//		res.SetPos(item, new Vector2Int(
		//			(int)Math.Round(posx), 
		//			(int)Math.Round(posy)));
		//		if (item == enPartsType.Ant)
		//			Debug.Log(frame.ToString() + " " + (int)Math.Round(posy));
		//	}
		//	return res;

		//}


		public void SetPos(enPartsType partsType, Vector2Int pos) {
			switch (partsType) {
				case enPartsType.Thorax: thorax.pos = pos; break;
				case enPartsType.Gaster: gaster.pos = pos; break;
				case enPartsType.Head: head.pos = pos; break;
				case enPartsType.LeftArm: leftArm.pos = pos; break;
				case enPartsType.LeftLeg: leftLeg.pos = pos; break;
				case enPartsType.RightArm: rightArm.pos = pos; break;
				case enPartsType.RightLeg: rightLeg.pos = pos; break;
				case enPartsType.Ant: ant.pos = pos; break;
			}
		}
		public Vector2Int GetPos(enPartsType partsType) {
			Vector2Int res = Vector2Int.zero;
			switch (partsType) {
				case enPartsType.Thorax: res = thorax.pos; break;
				case enPartsType.Gaster: res = gaster.pos; break;
				case enPartsType.Head: res = head.pos; break;
				case enPartsType.LeftArm: res = leftArm.pos; break;
				case enPartsType.LeftLeg: res = leftLeg.pos; break;
				case enPartsType.RightArm: res = rightArm.pos; break;
				case enPartsType.RightLeg: res = rightLeg.pos; break;
				case enPartsType.Ant: res = ant.pos; break;
			}
			return res;
		}

		public void SetCurveX(enPartsType partsType, enCurve curveX) {
			int intCurveX = (int)curveX;
			switch (partsType) {
				case enPartsType.Thorax: thorax.curveX = intCurveX; break;
				case enPartsType.Gaster: gaster.curveX = intCurveX; break;
				case enPartsType.Head: head.curveX = intCurveX; break;
				case enPartsType.LeftArm: leftArm.curveX = intCurveX; break;
				case enPartsType.LeftLeg: leftLeg.curveX = intCurveX; break;
				case enPartsType.RightArm: rightArm.curveX = intCurveX; break;
				case enPartsType.RightLeg: rightLeg.curveX = intCurveX; break;
				case enPartsType.Ant: ant.curveX = intCurveX; break;
			}
		}
		public enCurve GetCurveX(enPartsType partsType) {
			int res = 0;
			switch (partsType) {
				case enPartsType.Thorax: res = thorax.curveX; break;
				case enPartsType.Gaster: res = gaster.curveX; break;
				case enPartsType.Head: res = head.curveX; break;
				case enPartsType.LeftArm: res = leftArm.curveX; break;
				case enPartsType.LeftLeg: res = leftLeg.curveX; break;
				case enPartsType.RightArm: res = rightArm.curveX; break;
				case enPartsType.RightLeg: res = rightLeg.curveX; break;
				case enPartsType.Ant: res = ant.curveX; break;
			}
			return (enCurve)res;
		}

		public void SetCurveY(enPartsType partsType, enCurve curveY) {
			int intCurveY = (int)curveY;
			switch (partsType) {
				case enPartsType.Thorax: thorax.curveY = intCurveY; break;
				case enPartsType.Gaster: gaster.curveY = intCurveY; break;
				case enPartsType.Head: head.curveY = intCurveY; break;
				case enPartsType.LeftArm: leftArm.curveY = intCurveY; break;
				case enPartsType.LeftLeg: leftLeg.curveY = intCurveY; break;
				case enPartsType.RightArm: rightArm.curveY = intCurveY; break;
				case enPartsType.RightLeg: rightLeg.curveY = intCurveY; break;
				case enPartsType.Ant: ant.curveY = intCurveY; break;
			}
		}
		public enCurve GetCurveY(enPartsType partsType) {
			int res = 0;
			switch (partsType) {
				case enPartsType.Thorax: res = thorax.curveY; break;
				case enPartsType.Gaster: res = gaster.curveY; break;
				case enPartsType.Head: res = head.curveY; break;
				case enPartsType.LeftArm: res = leftArm.curveY; break;
				case enPartsType.LeftLeg: res = leftLeg.curveY; break;
				case enPartsType.RightArm: res = rightArm.curveY; break;
				case enPartsType.RightLeg: res = rightLeg.curveY; break;
				case enPartsType.Ant: res = ant.curveY; break;
			}
			return (enCurve)res;
		}

	}
}