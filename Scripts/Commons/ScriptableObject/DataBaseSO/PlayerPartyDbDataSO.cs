using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
// プレイヤーの味方データベース
public class PlayerPartyDbDataSO : ScriptableObject
{
    // 生存しているときの画像
    public Sprite AliveTexture;

    // 戦闘不能時の画像
    public Sprite DeadTexture;

    // キャラ図鑑に表示する全体画像
    public Sprite FullBodyTexture;

    // キャラ名
    public string playerName;

    // 各レベルにおけるステータステーブル
    public PlayerUpStatusTableSO playerUpStatusTableSO;

    // 各レベルにおける必要経験値テーブル
    public PlayerExpTableSO playerExpTableSO;

    // 現在のレベル
    public int Level;

    // 現在のHP
    public int HP;

    // 現在のTP
    public int TP;

    // 現在のレベルでの取得経験値
    public int Exp;

    // 火属性防御力
    public int FireDef;

    // 氷属性防御力
    public int WaterDef;

    // 風属性防御力
    public int WindDef;

    // 土属性防御力
    public int SoilDef;

    // 雷属性防御力
    public int ThunderDef;

    // キャラ説明
    public string story;

    // 習得スキル
    public List<SkillSO> skills = new();
}