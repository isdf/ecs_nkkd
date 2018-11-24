using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace NKKD
{
	[ExecuteInEditMode]
	public class Define : SingletonMonoBehaviour<Define>
	{
		public int MAP_GRID_NUM = 100;
		public int TIP_SIZE = 10;
		public int GRID_SIZE = 20;

		public int PLAYER_NUM = 1;
		public int CAHAR_NUM = 10;
		public List<TileBase> pheromTile;
		public int GetMapSize() { return (MAP_GRID_NUM * GRID_SIZE); }
	}

	//[CustomEditor(typeof(Define))]//拡張するクラスを指定
	//public class DefineEditor : Editor {
	//	public override void OnInspectorGUI() {
	//		//元のInspector部分を表示
	//		base.OnInspectorGUI();

	//		////ボタンを表示
	//		//if (GUILayout.Button("LoadObject")) {
	//		//	(target as AniScriptManager).LoadObject();
	//		//}
	//	}

	//}
}