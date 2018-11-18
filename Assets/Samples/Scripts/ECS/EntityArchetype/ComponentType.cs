// using Unity.Entities;
// using UnityEngine;
// using Unity.Collections;
// using Unity.Transforms;
// using Unity.Transforms2D;
// using Unity.Jobs;
// using Unity.Mathematics;
// using System.Collections.Generic;
// using Unity.Collections.LowLevel.Unsafe;
// using UnityEngine.Assertions;
// using UnityEngine.Experimental.PlayerLoop;
// using UnityEngine.Rendering;

// namespace NKKD {

// 	public struct CharaObj : IComponentData {}

// 	//アリの識別情報
// 	public struct CharaId : IComponentData {
// 		public int familyId;
// 		public int myId;
// 	}

// 	//アリの行動タイプ
// 	public struct CharaBehave : IComponentData {
// 		public int behaveType; //種類
// 		public float endTime; //終了予定時間
// 		public int angle; //目的方向
// 		public float2 targetVecNrm; //目的方向（正規化）
// 	}

// 	//アリの向きと表情
// 	public struct CharaLook : IComponentData {
// 		public int isLeft;
// 		public int isBack;
// 		public int faceNo;
// 	}

// 	//アリのモーション
// 	public struct CharaMotion : IComponentData {
// 		public int motionNo;
// 		public int count;
// 	}

// 	public struct TipObj : IComponentData { }

// 	//チップの座標
// 	public struct GridPos : IComponentData {
// 		public Vector2Int Value;
// 	}

// 	//チップの材質
// 	public struct TipSurface : IComponentData {
// 		public int surfType;
// 	}

// 	//フェロモン情報低揮発
// 	public struct TipPheromL : IComponentData {
// 		public int id;
// 		public int info;
// 		public float time;
// 		public Vector2Int vec;
// 	}

// 	//フェロモン情報高揮発
// 	public struct TipPheromH : IComponentData {
// 		public int id;
// 		public int info;
// 		public float time;
// 	}

// 	//たべものオブジェクト
// 	public struct FoodObj : IComponentData { }

// 	//データ
// 	public struct FoodData : IComponentData {
// 		public int foodType;
// 		public int volume;
// 	}

// 	//public struct GeoObj : IComponentData { }

// }