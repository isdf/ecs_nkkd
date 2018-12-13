﻿using System;
using System.Collections;
using UnityEngine;

namespace NKKD.EDIT
{
	public struct MotionPosState
	{
		public Vector2Int headPos;
		public Vector2Int thoraxPos;
		public Vector2Int gasterPos;
		public Vector2Int leftArmPos;
		public Vector2Int rightArmPos;
		public Vector2Int leftHandPos;
		public Vector2Int rightHandPos;
		public Vector2Int leftLegPos;
		public Vector2Int rightLegPos;
		public Vector2Int leftFootPos;
		public Vector2Int rightFootPos;
		public Vector2Int antPos;

		public void SetPos(enPartsType partsType, Vector2Int pos)
		{
			switch (partsType)
			{
				case enPartsType.Body:
					thoraxPos = pos;
					break;
				case enPartsType.Head:
					headPos = pos;
					break;
				case enPartsType.LeftArm:
					leftArmPos = pos;
					break;
				case enPartsType.RightArm:
					rightArmPos = pos;
					break;
				case enPartsType.LeftHand:
					leftHandPos = pos;
					break;
				case enPartsType.RightHand:
					rightHandPos = pos;
					break;
				case enPartsType.LeftLeg:
					leftLegPos = pos;
					break;
				case enPartsType.RightLeg:
					rightLegPos = pos;
					break;
				case enPartsType.LeftFoot:
					leftFootPos = pos;
					break;
				case enPartsType.RightFoot:
					rightFootPos = pos;
					break;
				case enPartsType.Ant:
					antPos = pos;
					break;
			}
		}

		public Vector2Int GetPos(enPartsType partsType)
		{
			Vector2Int res = Vector2Int.zero;
			switch (partsType)
			{
				case enPartsType.Body:
					res = thoraxPos;
					break;
				case enPartsType.Head:
					res = headPos;
					break;
				case enPartsType.LeftArm:
					res = leftArmPos;
					break;
				case enPartsType.RightArm:
					res = rightArmPos;
					break;
				case enPartsType.LeftHand:
					res = leftHandPos;
					break;
				case enPartsType.RightHand:
					res = rightHandPos;
					break;
				case enPartsType.LeftLeg:
					res = leftLegPos;
					break;
				case enPartsType.RightLeg:
					res = rightLegPos;
					break;
				case enPartsType.LeftFoot:
					res = leftFootPos;
					break;
				case enPartsType.RightFoot:
					res = rightFootPos;
					break;
				case enPartsType.Ant:
					res = antPos;
					break;
			}
			return res;
		}

		public AniFrame OutputFrame()
		{
			AniFrame res = new AniFrame();
			res.head = headPos;
			res.ant = antPos;
			res.body = thoraxPos;
			res.leftArm = leftArmPos;
			res.rightArm = rightArmPos;
			res.leftHand = leftHandPos;
			res.rightHand = rightHandPos;
			res.leftLeg = leftLegPos;
			res.rightLeg = rightLegPos;
			res.leftFoot = leftFootPos;
			res.rightFoot = rightFootPos;
			return res;
		}
	}
}