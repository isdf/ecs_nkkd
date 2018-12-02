using System;
using System.Collections.Generic;
using UnityEngine;
namespace NKKD.EDIT
{
	public static class BasePosition
	{
		public static readonly int GROUNDY = 7; //脚の地面からのドット数
		public static readonly int COREDY = 7; //体の中心
		public static readonly int HEADY = 7;
		public static readonly int ANTY = 11;
		public static readonly int ARMX = 2;
		public static readonly int LEGX = 2;
		public static readonly int GASTX = 6;
		public static readonly Vector2Int THORAXBASE_EDI = new Vector2Int(0, COREDY);
		public static readonly Vector2Int GASTERBASE_EDI = new Vector2Int(-GASTX, GROUNDY);
		public static readonly Vector2Int HEADBASE_EDI = new Vector2Int(0, (COREDY + HEADY));
		public static readonly Vector2Int LARMBASE_EDI = new Vector2Int(ARMX, COREDY);
		public static readonly Vector2Int RARMBASE_EDI = new Vector2Int(-ARMX, COREDY);
		public static readonly Vector2Int LLEGBASE_EDI = new Vector2Int(LEGX, GROUNDY);
		public static readonly Vector2Int RLEGBASE_EDI = new Vector2Int(-LEGX, GROUNDY);
		public static readonly Vector2Int ANTBASE_EDI = new Vector2Int(0, (COREDY + ANTY));

		public static readonly List<int> FRONTDEPTH = new List<int>
		{
			(int)enPartsType.LeftArm,
			(int)enPartsType.LeftLeg,
			(int)enPartsType.Gaster,
			(int)enPartsType.Thorax,
			(int)enPartsType.Head,
			(int)enPartsType.Ant,
			(int)enPartsType.RightLeg,
			(int)enPartsType.RightArm,
		};
		public static readonly List<int> BACKDEPTH = new List<int>
		{
			(int)enPartsType.LeftArm,
			(int)enPartsType.LeftLeg,
			(int)enPartsType.Ant,
			(int)enPartsType.Head,
			(int)enPartsType.Thorax,
			(int)enPartsType.Gaster,
			(int)enPartsType.RightLeg,
			(int)enPartsType.RightArm,
		};

		public static Vector2Int GetPosEdit(enPartsType partsType, bool isMirror)
		{
			Vector2Int res = Vector2Int.zero;
			switch (partsType)
			{
				case enPartsType.Thorax:
					res = THORAXBASE_EDI;
					break;
				case enPartsType.Gaster:
					res = GASTERBASE_EDI;
					break;
				case enPartsType.Head:
					res = HEADBASE_EDI;
					break;
				case enPartsType.Ant:
					res = ANTBASE_EDI;
					break;
				case enPartsType.LeftArm:
					res = (isMirror) ? RARMBASE_EDI : LARMBASE_EDI;
					break;
				case enPartsType.RightArm:
					res = (isMirror) ? LARMBASE_EDI : RARMBASE_EDI;
					break;
				case enPartsType.LeftLeg:
					res = (isMirror) ? RLEGBASE_EDI : LLEGBASE_EDI;
					break;
				case enPartsType.RightLeg:
					res = (isMirror) ? LLEGBASE_EDI : RLEGBASE_EDI;
					break;
			}
			return res;
		}

		public static Vector2Int GetPos(enPartsType partsType)
		{
			Vector2Int res = Vector2Int.zero;
			switch (partsType)
			{
				case enPartsType.Thorax:
					res = THORAXBASE_EDI;
					break;
				case enPartsType.Gaster:
					res = GASTERBASE_EDI;
					break;
				case enPartsType.Head:
					res = HEADBASE_EDI;
					break;
				case enPartsType.LeftArm:
					res = LARMBASE_EDI;
					break;
				case enPartsType.RightArm:
					res = RARMBASE_EDI;
					break;
				case enPartsType.LeftLeg:
					res = LLEGBASE_EDI;
					break;
				case enPartsType.RightLeg:
					res = RLEGBASE_EDI;
					break;
				case enPartsType.Ant:
					res = ANTBASE_EDI;
					break;
			}
			return res;
		}

		//public static float PartsAngleBaseZ(enPartsType partsType, enPartsAngle bodyAngle, bool isFlip) {
		//	float res = 0;
		//	float DEPTH = -0.001f;
		//	//描画順
		//	switch (bodyAngle) {
		//		case enPartsAngle.Look: res = LOOKDEPTH.IndexOf((int)partsType) * DEPTH; break;
		//		case enPartsAngle.Back: res = BACKDEPTH.IndexOf((int)partsType) * DEPTH; break;
		//		case enPartsAngle.Side: res = SIDEDEPTH.IndexOf((int)partsType) * DEPTH; break;
		//		case enPartsAngle.Front: res = FRONTDEPTH.IndexOf((int)partsType) * DEPTH; break;
		//		case enPartsAngle.Rear: res = REARDEPTH.IndexOf((int)partsType) * DEPTH; break;
		//	}

		//	//全体が反転してるときはスプライトボードが裏返っているので奥行きを反転
		//	if (isFlip) res = -res;

		//	return res;
		//}

		//ボディ向きから割り出した描画プライオリティリスト
		public static List<enPartsType> GenGetZSortList(bool isLeft, bool isBack)
		{
			List<enPartsType> res = new List<enPartsType>();
			if (isBack)
			{
				foreach (var item in BACKDEPTH)
				{
					enPartsType pt = (enPartsType)item;
					if (isLeft)
					{
						if (pt == enPartsType.LeftArm)pt = enPartsType.RightArm;
						else if (pt == enPartsType.RightArm)pt = enPartsType.LeftArm;
						else if (pt == enPartsType.LeftLeg)pt = enPartsType.RightLeg;
						else if (pt == enPartsType.RightLeg)pt = enPartsType.LeftLeg;
					}
					res.Add(pt);
				}
			}
			else
			{
				foreach (var item in FRONTDEPTH)
				{
					enPartsType pt = (enPartsType)item;
					if (isLeft)
					{
						if (pt == enPartsType.LeftArm)pt = enPartsType.RightArm;
						else if (pt == enPartsType.RightArm)pt = enPartsType.LeftArm;
						else if (pt == enPartsType.LeftLeg)pt = enPartsType.RightLeg;
						else if (pt == enPartsType.RightLeg)pt = enPartsType.LeftLeg;
					}
					res.Add(pt);
				}
			}
			////描画順
			//switch (bodyAngle) {
			//	case enPartsAngle.Look: foreach (var item in LOOKDEPTH) res.Add((enPartsType)item); break;
			//	case enPartsAngle.Back: foreach (var item in BACKDEPTH) res.Add((enPartsType)item); break;
			//	case enPartsAngle.Front: foreach (var item in FRONTDEPTH) res.Add((enPartsType)item); break;
			//	case enPartsAngle.Side: foreach (var item in SIDEDEPTH) res.Add((enPartsType)item); break;
			//	case enPartsAngle.Rear: foreach (var item in REARDEPTH) res.Add((enPartsType)item); break;
			//}
			return res;
		}

		////スクリプタブルオブジェクト用
		//public static float[] OutputDepth(enPartsAngle angle) {
		//	List<float> res = new List<float>();
		//	//描画順
		//	float DEPTH = -0.001f;
		//	switch (angle) {
		//		case enPartsAngle.Look: for (int i = 0; i < (int)enPartsType._END; i++) res.Add(LOOKDEPTH.IndexOf(i) * DEPTH); break;
		//		case enPartsAngle.Back: for (int i = 0; i < (int)enPartsType._END; i++) res.Add(BACKDEPTH.IndexOf(i) * DEPTH); break;
		//		case enPartsAngle.Front: for (int i = 0; i < (int)enPartsType._END; i++) res.Add(FRONTDEPTH.IndexOf(i) * DEPTH); break;
		//		case enPartsAngle.Side: for (int i = 0; i < (int)enPartsType._END; i++) res.Add(SIDEDEPTH.IndexOf(i) * DEPTH); break;
		//		case enPartsAngle.Rear: for (int i = 0; i < (int)enPartsType._END; i++) res.Add(REARDEPTH.IndexOf(i) * DEPTH); break;
		//	}
		//	return res.ToArray();
		//}

		//スクリプタブルオブジェクト用
		public static AniDepth OutputDepth(bool isBack)
		{
			List<float> res = new List<float>();
			//描画順
			float DEPTH = -0.0001f; //やっぱマイナスが上
			if (isBack)
			{
				for (int i = 0; i < (int)enPartsType._END; i++)res.Add(BACKDEPTH.IndexOf(i) * DEPTH);
			}
			else
			{
				for (int i = 0; i < (int)enPartsType._END; i++)res.Add(FRONTDEPTH.IndexOf(i) * DEPTH);
			}
			AniDepth res2 = new AniDepth();
			res2.SetData(res.ToArray());
			return res2;
		}

	}
}