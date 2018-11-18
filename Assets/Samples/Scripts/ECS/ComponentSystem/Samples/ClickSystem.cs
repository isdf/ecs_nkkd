using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[AlwaysUpdateSystem]
sealed class ClickSystem : ComponentSystem
{
    EntityArchetype entityArchetype;
    ComponentGroup g;
    protected override void OnCreateManager()
    {
        // ComponentType構造体は必ずジェネリックメソッドを使用して作成するべきである。
        // Type型を引数に取るオーバーロードやType型からの暗黙的型変換も作成方法としてあるが、
        // 線形探索のためパフォーマンスが頗る悪い。
        // タグ用途のComponentDataは必ずReadOnly<T>()で作成するべきである。
        // ComponentType.ReadOnly(typeof(Count))はNG
        var componentTypes = new ComponentType[] { ComponentType.ReadOnly<Count>() };

        // Entityを作る際に最初から持つべきComponentTypeを設定する。
        entityArchetype = EntityManager.CreateArchetype(componentTypes);

        // 引数に与えられたComponentTypeと一致するEntityのみを処理対象とするComponentGroupを得る。
        // Getと書いてあるが実際はGetOrCreate相当の働きを持つ。
        g = GetComponentGroup(componentTypes);
    }

    protected override void OnUpdate()
    {
        if (Input.GetMouseButton(0))
            EntityManager.CreateEntity(entityArchetype);
        else if (Input.GetMouseButton(1))
        {
            var source = g.GetEntityArray();
            if (source.Length == 0)
                return;
            using(var results = new NativeArray<Entity>(source.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory))
            {
                new CopyEntities
                {
                    Results = results,
                        Source = source,
                }.Schedule(source.Length, 256).Complete();
                EntityManager.DestroyEntity(results);
            }
        }
    }
}