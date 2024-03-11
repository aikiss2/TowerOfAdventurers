using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// イベント実行データベース
public class EventMemoryDbSO : ScriptableObject
{
    // 実施済みかフラグ(true:実施済み)。EventTableSOと同期させること。
    public List<bool> done = new();
}