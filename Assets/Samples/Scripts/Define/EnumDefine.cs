using System;
using System.Collections.Generic;
using UnityEngine;
namespace NKKD
{

	public enum EnumPartsTypeBase
	{
		Ant,
		Head,
		Thorax,
		Gaster,
		Arm,
		Leg,
		_END,
	}

	//パーツ位置
	public enum EnumPartsType
	{
		Ant = 0,
		Head,
		Thorax,
		Gaster,
		LeftArm,
		RightArm,
		LeftLeg,
		RightLeg,
		_END,
		};

		public enum EnumBehave
		{
		Idle,
		Wander,
		Goto,
		Attack,
		_END,
	}

	//パーツ位置
	public enum EnumMotion
	{
		Idle = 0,
		Walk,
		//_END,
		};
}