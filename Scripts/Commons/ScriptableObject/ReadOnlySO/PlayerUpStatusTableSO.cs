using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
// 味方キャラクターのレベルによるステータステーブル
public class PlayerUpStatusTableSO : ScriptableObject
{
    // リストの配列順番がレベルに対応
    public List<PlayerUpStatusData> playerUpStatusTableData = new();
}

[System.Serializable]
// レベル(配列番号)に応じたステータス
public class PlayerUpStatusData
{
    public int MaxHP;    // 最大HP
    public int MaxTP;    // 最大TP
    public int ATK;      // 攻撃力
    public int DEF;      // 防御力
    public int INT;      // 魔法攻撃力
    public int RES;      // 回復力
    public int DEX;      // 命中力
    public int AGI;      // 素早さ
}