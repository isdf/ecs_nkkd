using TMPro;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// ECSのセットアップを行うManager_CountUp Component
/// </summary>
sealed class Manager_CountUp : MonoBehaviour
{
    [SerializeField] TMP_Text countText;
    void Start()
    {
        // Worldの作成
        var world = World.Active = new World("count up");
        // ComponentSystemの初期化
        // CountUpSystemのpublicコンストラクタに引数を渡せる。
        world.CreateManager(typeof(CountUpSystem), countText);
        world.CreateManager(typeof(ClickSystem));

        // PlayerLoopへのWorldの登録
        ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
    }
}