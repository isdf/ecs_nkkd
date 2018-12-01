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
			public int CAHAR_NUM = 1;
			public List<TileBase> pheromTile;
			public int GetMapSize() { return (MAP_GRID_NUM * GRID_SIZE); }
		}
}