using UnityEngine;
using System;
using System.Collections;

namespace NKKD.EDIT
{
	public struct PartsTransformState
	{
		public int rotate;


		public void SetFromPartsTransform(PartsTransform partsTransform)
		{
			rotate = partsTransform.rotate;
			//isLeft = partsTransform.isLeft;
			//isBack = partsTransform.isBack;
		}

		public void Reset()
		{
			this = new PartsTransformState();
		}
	}

	public struct MotionTransformState {
		public PartsTransformState head;
		public PartsTransformState thorax;
		public PartsTransformState gaster;
		public PartsTransformState leftArm;
		public PartsTransformState rightArm;
		public PartsTransformState leftLeg;
		public PartsTransformState rightLeg;
		public PartsTransformState ant;

		public PartsTransformState GetTransform(enPartsType partsType) {
			PartsTransformState res = new PartsTransformState();
			switch (partsType) {
				case enPartsType.Thorax: res = thorax; break;
				case enPartsType.Gaster: res = gaster; break;
				case enPartsType.Head: res = head; break;
				case enPartsType.LeftArm: res = leftArm; break;
				case enPartsType.LeftLeg: res = leftLeg; break;
				case enPartsType.RightArm: res = rightArm; break;
				case enPartsType.RightLeg: res = rightLeg; break;
				case enPartsType.Ant: res = ant; break;
			}
			return res;
		}

		public void SetFromPartsTransform(enPartsType partsType, PartsTransform partsTransform) {
			switch (partsType) {
				case enPartsType.Thorax: thorax.SetFromPartsTransform(partsTransform); break;
				case enPartsType.Gaster: gaster.SetFromPartsTransform(partsTransform); break;
				case enPartsType.Head: head.SetFromPartsTransform(partsTransform); break;
				case enPartsType.LeftArm: leftArm.SetFromPartsTransform(partsTransform); break;
				case enPartsType.LeftLeg: leftLeg.SetFromPartsTransform(partsTransform); break;
				case enPartsType.RightArm: rightArm.SetFromPartsTransform(partsTransform); break;
				case enPartsType.RightLeg: rightLeg.SetFromPartsTransform(partsTransform); break;
				case enPartsType.Ant: ant.SetFromPartsTransform(partsTransform); break;
			}
		}
	}
}