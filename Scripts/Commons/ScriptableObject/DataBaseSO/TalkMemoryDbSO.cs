using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 会話実行データベース
public class TalkMemoryDbSO : ScriptableObject
{
    // 実施済みかフラグ(true:実施済み)。TalkTableSOと同期させること。
    public List<bool> done = new();
}