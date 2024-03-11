using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
// バトルでエンカウントする敵のテーブル
public class CreateEnemyTableSO : ScriptableObject
{
    public List<CreateEnemyData> createEnemyTableData = new();
}

[System.Serializable]
// 敵データ
public class CreateEnemyData
{
    // 敵の番号(EnemyStatusTableSOのリスト番号と同期)
    [field: SerializeField] public int EnemyNumber { get; private set; }
}