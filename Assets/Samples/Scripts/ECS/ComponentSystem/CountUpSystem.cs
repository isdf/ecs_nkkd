using TMPro;
using Unity.Entities;
using Unity.Jobs;

[AlwaysUpdateSystem]
sealed class CountUpSystem : ComponentSystem
{
    // uGUI標準Textだと文字が滲むのでTextMeshProを使用。
    readonly TMP_Text countDownText;
    ComponentGroup g;
    uint cachedCount = 0;
    // GetComponentGroupはコンストラクタから呼べないため、OnCreateManagerで呼び出すべし。
    public CountUpSystem(TMP_Text countDownText)
    {
        this.countDownText = countDownText;
    }
    protected override void OnCreateManager() => g = GetComponentGroup(ComponentType.ReadOnly<Count>());

    protected override void OnUpdate()
    {
        uint current = (uint)g.CalculateLength();
        if (current == cachedCount)return;
        cachedCount = current;
        countDownText.text = cachedCount.ToString();
    }
}