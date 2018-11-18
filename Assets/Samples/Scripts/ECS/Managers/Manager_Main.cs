using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

namespace NKKD
{

    /// <summary>
    /// ECSのセットアップを行うManager_Main Component
    /// </summary>
    sealed class Manager_Main : MonoBehaviour
    {

        // ワールドとシーン名を一致させる
        const string SCENE_NAME = "Main";

        const int ARINUM = 10;

        // // エンティティリスト
        // List<Entity> m_entityList;
        // // エンティティマネージャー
        // EntityManager entityManager;
        // [SerializeField] MeshInstanceRenderer meshInstanceRenderer;
        // [SerializeField] uint count;
        // [SerializeField] uint2 range;
        // [SerializeField] uint danceLoopLength;
        void Start()
        {
            //シーンの判定
            var scene = SceneManager.GetActiveScene();
            if (scene.name != SCENE_NAME)
                return;

            // ワールド生成
            var manager = InitializeWorld();

            //SharedComponentDataの準備
            ReadySharedComponentData();

            //コンポーネントのキャッシュ
            ComponentCache();

            // エンティティ生成
            InitializeEntities(manager);
        }

        /// <summary>
        /// ワールド生成
        /// </summary>
        /// <returns></returns>
        EntityManager InitializeWorld()
        {
            var worlds = new World[1];
            ref
            var world = ref worlds[0];

            world = new World(SCENE_NAME);
            var manager = world.CreateManager<EntityManager>();

            // ComponentSystemの初期化
            InitializeSystem(world);

            // PlayerLoopへのWorldの登録
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(worlds);

            return manager;
        }

        /// <summary>
        /// ComponentSystemの初期化
        /// </summary>
        /// <param name="world"></param>
        void InitializeSystem(World world)
        {
            // Systemは World.Active.GetOrCreateManager<EntityManager>() で明示的に作成できます。
            // World.Active.CreateManager<EntityManager>() もありますが、 
            // 既にEntityが存在すると問題になるので、 個人的には「 明示的な作成」 のような
            // 生成タイミングがフワっとしてるものにはオススメしません。（ Worldの初期化時に作るんだ!!!等ならCreateManagerでも）
            // world.CreateManager(typeof(MoveSystem));
            // world.CreateManager(typeof(EndFrameBarrier));
            // world.CreateManager(typeof(DanceSystem));
            // world.CreateManager(typeof(EndFrameTransformSystem));
            // world.CreateManager<MeshInstanceRendererSystem>().ActiveCamera = GetComponent<Camera>();

            world.CreateManager(typeof(BehaveMoveSystem));
            world.CreateManager(typeof(ChangeBehaveSystem));
            world.CreateManager(typeof(MotionCountSystem));
            world.CreateManager(typeof(CharaDrawSystem));

        }

        //各コンポーネントのキャッシュ
        void ComponentCache()
        {
            Cache.pixelPerfectCamera = FindObjectOfType<PixelPerfectCamera>();
            //Cache.tilemap = FindObjectOfType<Tilemap>();
            var tileMaps = FindObjectsOfType<Tilemap>();
            foreach (var item in tileMaps)
            {
                //Debug.Log(item.layoutGrid.name);
                if (item.layoutGrid.name == "PheromGrid")
                {
                    Cache.pheromMap = item;
                    Cache.pheromMap.ClearAllTiles();
                    Cache.pheromMap.size = new Vector3Int(Define.Instance.GRID_SIZE, Define.Instance.GRID_SIZE, 0);
                }
            }
        }

        //SharedComponentDataの読み込み
        void ReadySharedComponentData()
        {
            Shared.ReadySharedComponentData();
        }

        /// <summary>
        /// エンティティ生成
        /// </summary>
        /// <param name="manager"></param>
        void InitializeEntities(EntityManager manager)
        {
            //アリ作成
            CreateCharaEntity(manager);

            //             if (count == 0)
            //                 return;
            //             var entities = new NativeArray<Entity>((int)count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            //             try
            //             {
            //                 entities[0] = manager.CreateEntity(ComponentType.Create<Position>(),
            //                     ComponentType.Create<MeshInstanceRenderer>(),
            //                     ComponentType.Create<StartTime>(),
            //                     ComponentType.Create<Velocity>(),
            //                     ComponentType.Create<DanceMove>(),
            //                     ComponentType.Create<DanceSystem.Tag>());
            //                 manager.SetSharedComponentData(entities[0], meshInstanceRenderer);
            //                 manager.SetComponentData(entities[0], new StartTime { Value = Time.timeSinceLevelLoad });
            //                 unsafe
            //                 {
            //                     var rest = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Entity>(((Entity * )NativeArrayUnsafeUtility.GetUnsafePtr(entities)) + 1, entities.Length - 1, Allocator.Temp);
            // #if ENABLE_UNITY_COLLECTIONS_CHECKS
            //                     NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref rest, NativeArrayUnsafeUtility.GetAtomicSafetyHandle(entities));
            // #endif
            //                     manager.Instantiate(entities[0], rest);
            //                 }
            //                 var rand = new Unity.Mathematics.Random((uint)DateTime.Now.Ticks);
            //                 for (int i = 0; i < entities.Length; i++)
            //                     InitializeEntity(ref rand, manager, entities[i]);
            //             }
            //             finally
            //             {
            //                 entities.Dispose();
            //             }
        }

        //アリエンティティ作成
        void CreateCharaEntity(EntityManager manager)
        {
            //アリエンティティ作成
            for (int i = 0; i < ARINUM; i++)
            {
                var entity = CharaEntityFactory.CreateEntity(i, manager,
                    ref Shared.ariMeshMat, ref Shared.aniScriptSheet, ref Shared.aniBasePos);
                //エンティティリストに追加
                // entityList.Add(entity);
            }
        }

        // void InitializeEntity(ref Unity.Mathematics.Random random, EntityManager manager, Entity entity)
        // {
        //     var moves = manager.GetBuffer<DanceMove>(entity);
        //     DanceMove move = default;
        //     for (uint i = 0; i < danceLoopLength; ++i)
        //     {
        //         var values = random.NextFloat4() * 10f - 5f;
        //         move.Duration = values.w + 5f;
        //         move.Velocity = values.xyz;
        //         random.state = random.NextUInt();
        //         moves.Add(move);
        //     }
        //     manager.SetComponentData(entity, new Velocity { Value = (random.NextFloat3() - 0.5f) * 8f });
        // }
    }
}