using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敵キャラクターのステータステーブル
public class EnemyStatusTableSO : ScriptableObject
{
    public List<EnemyStatusData> enemyStatusTableData = new List<EnemyStatusData>();
}

[System.Serializable]
// 敵キャラクターのステータスデータ
public class EnemyStatusData
{
    [field: SerializeField] public string Name { get; private set; }         // 名称
    [field: SerializeField] public Sprite Texture { get; private set; }      // イメージ
    [field: SerializeField] public int Level { get; private set; }           // レベル
    [field: SerializeField] public int MaxHP { get; private set; }           // 最大HP
    [field: SerializeField] public int MaxTP { get; private set; }           // 最大TP
    [field: SerializeField] public int ATK { get; private set; }             // 攻撃力
    [field: SerializeField] public int DEF { get; private set; }             // 防御力
    [field: SerializeField] public int INT { get; private set; }             // 魔法攻撃力
    [field: SerializeField] public int RES { get; private set; }             // 回復力
    [field: SerializeField] public int DEX { get; private set; }             // 命中力
    [field: SerializeField] public int AGI { get; private set; }             // 素早さ
    [field: SerializeField] public int FireDef { get; private set; }         // 火属性防御力
    [field: SerializeField] public int WaterDef { get; private set; }        // 氷属性防御力
    [field: SerializeField] public int WindDef { get; private set; }         // 風属性防御力
    [field: SerializeField] public int SoilDef { get; private set; }         // 土属性防御力
    [field: SerializeField] public int ThunderDef { get; private set; }      // 雷属性防御力
    [field: SerializeField] public int Exp { get; private set; }             // 倒されたときの経験値
    [field: SerializeField] public int DropGold { get; private set; }        // 倒されたときのゴールド
    [field: SerializeField] public string Story { get; private set; }        // キャラ説明

    // 使用スキル
    [field: SerializeField] public List<SkillSO> Skills { get; private set; } = new();

    // 各使用スキルの使用確率(上記skillsと順番を同期させること)
    [field: SerializeField] public List<int> EnemySkillOdds { get; private set; } = new();
}