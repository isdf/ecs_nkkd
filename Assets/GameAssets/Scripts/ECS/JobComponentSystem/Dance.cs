﻿using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public sealed class DanceSystem : JobComponentSystem
{
    public struct Tag : IComponentData
    {
        public int Value;
    }
    readonly EntityArchetypeQuery query = new EntityArchetypeQuery
    {
        // 必ずEntityArchetypeQueryのフィールドに配列を初期化してあげてください。 nullのままだとエラー吐きます(preview11時点で確認)。
        // Any
        // 少なくともいずれか１つのComponentTypeを含む

        // None
        // この配列内のComponentTypeを含まない(ComponentGroupのSubtractive相当)

        // All
        // 必ずこの配列内のComponentType全てを含む

        // ComponentTypeを指定しない場合はそのフィールドにSystem.Array.Empty<ComponentType>() を代入してください。 new ComponentType[0] はメモリ的に非効率で非推奨です。

        Any = Array.Empty<ComponentType>(),
        None = Array.Empty<ComponentType>(),
        All = new [] { ComponentType.Create<Position>(), ComponentType.Create<DanceMove>() },
    };
    readonly NativeList<EntityArchetype> founds = new NativeList<EntityArchetype>(Allocator.Persistent);
    protected override void OnDestroyManager()
    {
        founds.Dispose();
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps) => new Job
    {
        bufferFrom = GetBufferFromEntity<DanceMove>(true),
            current = Time.timeSinceLevelLoad,
    }.Schedule(this, inputDeps);

    [Unity.Burst.BurstCompile]
    [RequireComponentTag(typeof(DanceMove))]
    struct Job : IJobProcessComponentDataWithEntity<Position, StartTime, Velocity, Tag>
    {
        [ReadOnly] public BufferFromEntity<DanceMove> bufferFrom;
        public float current;
        public void Execute(Entity entity, int index, ref Position pos, ref StartTime time, ref Velocity velocity, ref Tag tag)
        {
            var moves = bufferFrom[entity];
            if (moves.Length <= tag.Value)return;
            if (current > moves[tag.Value].Duration + time.Value)
            {
                ++tag.Value;
                tag.Value %= moves.Length;
                time.Value = current;
            }
            velocity = moves[tag.Value].Velocity;
        }
    }
}