using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 味方キャラクターの各レベルにおける必要経験値テーブル
public class PlayerExpTableSO : ScriptableObject
{
    // リストの配列順番がレベルに対応
    public List<PlayerExpData> playerExpTableData = new();
}

[System.Serializable]
// レベル(配列番号)に応じた必要経験値データ
public class PlayerExpData
{
    // 次のレベルに必要な経験値
    [field: SerializeField] public int NeedExp { get; private set; }
}