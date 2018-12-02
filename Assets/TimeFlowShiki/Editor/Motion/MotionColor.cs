using System;
using System.Collections;
using UnityEngine;

namespace NKKD.EDIT
{

	//タイムライン：パーツアニメーション
	[Serializable]
	public struct MotionColor
	{
		public int palette; //パレットアニメーション
		public int alphaAni; //透過アニメーション
		public int alphaVar; //透過引数

		public bool isAnt;
		public bool isHead;
		public bool isThorax;
		public bool isGaster;
		public bool isLeftArm;
		public bool isLeftLeg;
		public bool isRightArm;
		public bool isRightLeg;

		public bool IsActive(enPartsType partsType)
		{
			bool res = false;
			switch (partsType)
			{
				case enPartsType.Ant:
					res = isAnt;
					break;
				case enPartsType.Head:
					res = isHead;
					break;
				case enPartsType.Thorax:
					res = isThorax;
					break;
				case enPartsType.Gaster:
					res = isGaster;
					break;
				case enPartsType.LeftArm:
					res = isLeftArm;
					break;
				case enPartsType.LeftLeg:
					res = isLeftLeg;
					break;
				case enPartsType.RightArm:
					res = isRightArm;
					break;
				case enPartsType.RightLeg:
					res = isRightLeg;
					break;
			}
			return res;
		}
	}
}