using System.Collections.Generic;
using UnityEngine;
using static SkillSO;

// define定義
public static class Define
{
    // HPの上限/下限
    public const int HP_MAX_LIMIT = 999;
    public const int HP_MIN_LIMIT = 0;

    // TPの上限/下限
    public const int TP_MAX_LIMIT = 999;
    public const int TP_MIN_LIMIT = 0;

    // キャラクターレベルの最大
    public const int BATTLE_CHARACTER_LEVEL_MAX = 30;

    // マップでの１タイルのサイズ(１辺の長さ)
    public const float MAP_TILE_SIZE = 0.64f;

    // マップでの衝突判定に用いる距離
    public const float MAP_OBJECT_COLLISION_DISTANCE = 0.2f;

    // ダンジョンでの敵の数の最大
    public const int DUNGEON_ENEMY_MAX = 20;

    // ダンジョンでの敵生成の試行回数の最大
    public const int DUNGEON_CREATE_ENEMY_TRY_MAX = 10;

    // ダンジョンでの１歩移動時の敵生成確率
    public const int DUNGEON_CREATE_ENEMY_RATE = 10;

    // ダンジョンでの敵生成時のプレイヤー距離の最小マス数(X方向)
    public const int DUNGEON_CREATE_ENEMY_DISTANCE_X_MIN = 5;

    // ダンジョンでの敵生成時のプレイヤー距離の最小マス数(X方向)
    public const int DUNGEON_CREATE_ENEMY_DISTANCE_X_MAX = 9;

    // ダンジョンでの敵生成時のプレイヤー距離の最小マス数(Y方向)
    public const int DUNGEON_CREATE_ENEMY_DISTANCE_Y_MIN = 4;

    // ダンジョンでの敵生成時のプレイヤー距離の最小マス数(Y方向)
    public const int DUNGEON_CREATE_ENEMY_DISTANCE_Y_MAX = 8;

    // ダンジョンでのバトル遷移時の指定シーン番号
    public const int DUNGEON_NEXT_BATTLE_SCENE_NO = 5;

    // バトルでの敵の数の最大
    public const int BATTLE_ENEMY_MAX = 3;

    // バトルでの１回の攻撃でのダメージ最小値
    public const int BATTLE_DAMAGE_MIN = 1;

    // バトルでの１回の攻撃でのダメージ最大値
    public const int BATTLE_DAMAGE_MAX = 9999;

    // バトルでの攻撃時のミス確率の最大
    public const int BATTLE_ATTACK_MISS_RATE_MAX = 50;

    // バトルでの攻撃時のクリティカル確率の最大
    public const int BATTLE_ATTACK_CRITICAL_RATE_MAX = 50;

    // バトルでの逃げられる確率
    public const int BATTLE_ESCAPE_RATE = 80;

    // スキル名の最大文字数
    public const int SKILL_NAME_LENGTH_MAX = 9;

    // スキル消費TPの最大桁数
    public const int SKILL_TP_LENGTH_MAX = 2;

    // スキル値段の最大桁数
    public const int SKILL_GOLD_LENGTH_MAX = 5;

    // ゲームのフレームレート
    public const int GAME_FRAME_RATE = 60;

    // バフの最大レート
    public const float BATTLE_BUFF_MAX = 1.3f;

    // バフの最小レート(デバフ)
    public const float BATTLE_BUFF_MIN = 0.7f;
}


// ストーリー進行管理フラグ
public enum StoryStage
{
    None,
    Dungeon1,               // ダンジョン1
    Dungeon1_Player3Add,    // ダンジョン1で3人目の味方加入
}


// 特定イベント後の会話管理フラグ
public enum EventTalk
{
    None,
    GameOver,               // 全滅後の会話
    Player1_Dead,           // 味方1戦闘不能時
    Player1_Rebirth,        // 味方1戦闘不能から蘇生時
    Player2_Dead,           // 味方2戦闘不能時
    Player2_Rebirth,        // 味方2戦闘不能から蘇生時
    Player3_Dead,           // 味方3戦闘不能時
    Player3_Rebirth,        // 味方3戦闘不能から蘇生時
    GetItem,                // アイテム取得
}


// 会話テーブルのインデックス
public enum TalkTableIndex
{
    None,
    Continue,               // 一連の会話を継続する
    Town_Talk,              // 町での「会話」コマンド
    Town1,
    Town2,
    Town3,
    Town4,
    Town5,
    Town6,
    Town7,
    Town8,
    Town9,
    Town10,
    Sukiruya_Talk,          // スキル屋での「会話」コマンド
    Sukiruya1,
    Sukiruya2,
    Sukiruya3,
    Sukiruya4,
    Sukiruya5,
    Sukiruya6,
    Sukiruya7,
    Sukiruya8,
    Sukiruya9,
    Sukiruya10,
    Yadoya_Talk,            // 宿屋での「会話」コマンド
    Yadoya1,
    Yadoya2,
    Yadoya3,
    Yadoya4,
    Yadoya5,
    Yadoya6,
    Yadoya7,
    Yadoya8,
    Yadoya9,
    Yadoya10,
    Dungeon1_Talk,          // ダンジョン1での「会話」コマンド
    Dungeon1_Boss,
    Dungeon1_1,
    Dungeon1_2,
    Dungeon1_3,
    Dungeon1_4,
    Dungeon1_5,
    Dungeon1_6,
    Dungeon1_7,
    Dungeon1_8,
    Dungeon1_9,
    Dungeon1_10,
    Dungeon2_Talk,
    Dungeon2_Boss,
    Dungeon2_1,
    Dungeon2_2,
    Dungeon2_3,
    Dungeon2_4,
    Dungeon2_5,
    Dungeon2_6,
    Dungeon2_7,
    Dungeon2_8,
    Dungeon2_9,
    Dungeon2_10,
    Dungeon3_Talk,
    Dungeon3_Boss,
    Dungeon3_1,
    Dungeon3_2,
    Dungeon3_3,
    Dungeon3_4,
    Dungeon3_5,
    Dungeon3_6,
    Dungeon3_7,
    Dungeon3_8,
    Dungeon3_9,
    Dungeon3_10,
    Dungeon1_11,
    Dungeon1_12,
    Dungeon1_13,
    Dungeon1_14,
    Dungeon1_15,
    Dungeon1_16,
    Dungeon1_17,
    Dungeon1_18,
    Dungeon1_19,
    Dungeon1_20,
}


// 会話キャラ
public enum TalkChara
{
    Player1,
    Player2,
    Player3,
    Yadoya,
    Sukiruya,
    Mob1,
    Mob2,
    Mob3,
    Boss1,
    Boss2,
    Boss3,
    Boss4,
    Player1and2,
}


// 会話キャラを表示させるウィンドウ
public enum TalkSide
{
    None,
    Left,           // 左ウィンドウ
    Right,          // 右ウィンドウ
    Both,           // 左右両方のウィンドウ
}


// ゲームシーン
public enum SceneName
{
    None,
    Title,
    Town,
    Dungeon1_1,
    Dungeon1_2,
    Dungeon1_3,
    Dungeon2_1,
    Dungeon2_2,
    Dungeon2_3,
    Dungeon3_1,
    Dungeon3_2,
    Dungeon3_3,
    Battle,
    Yadoya,
    Sukiruya
}


// イベント管理インデックス
public enum EventTableIndex
{
    None,
    Dungeon1_1,
    Dungeon1_2,
    Dungeon1_3,
    Dungeon1_4,
    Dungeon1_5,
    Dungeon1_6,
    Dungeon1_7,
    Dungeon1_8,
    Dungeon1_9,
    Dungeon1_10,
    Dungeon1_11,
    Dungeon1_12,
    Dungeon1_13,
    Dungeon1_14,
    Dungeon1_15,
    Dungeon1_16,
    Dungeon1_17,
    Dungeon1_18,
    Dungeon1_19,
    Dungeon1_20,
    Dungeon1_21,
    Dungeon1_22,
    Dungeon1_23,
    Dungeon1_24,
    Dungeon1_25,
    Dungeon1_26,
    Dungeon1_27,
    Dungeon1_28,
    Dungeon1_29,
    Dungeon1_30,
}


// イベント種別(EventTaskクラスのeventTaskListと順番を同期させる)
public enum EventType
{
    GetGold,            // 宝箱からゴールド取得
    GetSkill,           // 宝箱からスキル取得
    Talk,               // 会話
    ObjectDestroy,      // キャラやオブジェクトを削除
    PlayerAdd,          // 味方キャラが仲間になる
    PlayBgm,            // BGM再生
    PlaySe,             // SE鳴らす
    Fadeout,            // フェードアウト
    UI_Hide,            // UIを隠す
    GetLicense,         // 冒険証取得
    GameClear,          // ゲームクリア
}


// スキル全種
public enum SkillTable
{
    None,
    Cut1,
    Cut2,
    Cut3,
    Cut4,
    Cut5,
    Cut6,
    Cut7,
    Cut8,
    Cut9,
    Cut10,
    Thrust1,
    Hit1,
    Hit2,
    Fire1,
    Fire2,
    Thunder1,
    Soil1,
    Normal1,
    Buff1,
    Buff2,
    Buff3,
    Buff4,
    Heal1,
    Heal2,
    Heal3,
    Heal4,
    Escape1,
    Water1,
    Water2,
    Wind1,
}


// スキルのストック状況
public enum StockCondition
{
    NoGet,      // 未入手
    NoBuy,      // 未購入
    Stocked,    // 所持
    Use,        // 装備中
}


// ダンジョンの敵キャラ記憶情報
public struct MapEnemyMemory
{
    public int power;
    public Vector3 pos;
}


// ダンジョンのキャラ記憶情報
public struct MapCharaMemory
{
    public Vector3 pos;
    public Vector2 direction;
}


// シーン移動時のオブジェクト識別子
public enum SceneChangeObjectNumber
{
    Number0,
    Number1,
    Number2,
    Number3,
    Number4
}


// 戦闘キャラのタイプ
public enum BattlerType
{
    Player,    //プレイヤー
    Enemy,     //敵
}


// 攻撃種別
public enum AttackType
{
    None,
    Physical,   // 物理
    Element,    // 魔法
}


// 物理種別
public enum PhysicalType
{
    None,
    Cut,        // 斬撃
    Thrust,     // 突き
    Hit,        // 打撃
}


// 魔法種別
public enum ElementType
{
    None,
    Fire,       // 火
    Water,      // 水(氷)
    Wind,       // 風
    Soil,       // 土
    Thunder,    // 雷
}


// 戦闘キャラのステータス
public enum StatusType
{
    HP,         // 体力
    MaxHP,      // 体力上限
    TP,         // TP
    MaxTP,      // TP上限
    ATK,        // 攻撃力
    DEF,        // 防御力
    INT,        // 魔法攻撃力
    RES,        // 魔法回復力
    DEX,        // 命中力
    AGI,        // 速さ
}


// スキル対象
public enum TargetType
{
    Self,               // 自身
    OpponentChoose,     // 敵(単一)
    OpponentAll,        // 敵(全体)
    FriendAll,          // 味方全体
    OpponentRandom,     // 敵(ランダム)
    FriendChoose,       // 味方から選ぶ
    DeadFriendChoose,   // 死亡した味方から選ぶ
}


// 攻撃や回復の量を決める基準種別
public enum VolumeType
{
    Ability,        // 能力
    Ratio,          // 割合
    StaticValue,    // 固定値
}


// プレイヤー入力種別
public enum PlayerInput
{
    None,
    Decide, // 決定
    Cancel, // キャンセル
}


// 汎用クラス
public sealed class Utility
{
    // 前シーンの名前(どのシーンから遷移してきたか管理)
    public static SceneName beforeSceneName;

    // 次シーンのオブジェクト番号(主にダンジョンで使用。プレイヤーの開始地点の情報)
    public static SceneChangeObjectNumber nextSceneObjectNumber;

    // 戦闘終了後のイベント指定
    public static EventTableIndex nextEvent = EventTableIndex.None;

    // ダンジョンのプレイヤー位置記憶
    public static MapCharaMemory mapPlayerMemory;

    // ダンジョンの敵キャラ記憶リスト
    public static List<MapEnemyMemory> mapEnemiesMemory = new ();

    // ダンジョンのNPCキャラ位置記憶リスト
    public static List<MapCharaMemory> mapCharactersMemory = new ();

    // 戦闘時のパーティ全体の取得経験値
    public static int getPlayersExp;

    // 戦闘時の取得ゴールド
    public static int getGold;

    // 次シーン
    public static SceneName nextScene;


    // スキル対象のリストを取得する
    public static List<BattleCharacter> GetTargetList(TargetType targetType, BattleCharacter target, List<BattleCharacter> friends, List<BattleCharacter> opponents)
    {
        List<BattleCharacter> targets = new ();

        //相手全員
        if (targetType == TargetType.OpponentAll)
        {
            targets = opponents;
        }
        // 味方全員
        else if (targetType == TargetType.FriendAll)
        {
            targets = friends;
        }
        // 味方選択
        else if (targetType == TargetType.FriendChoose)
        {
            // 対象が既に削除済みの場合
            if(target == null)
            {
                // ランダムに選択
                int randomCount = Random.Range(0, friends.Count);
                targets.Add(friends[randomCount]);
            }
            // 対象が戦闘不能
            else if (target.IsDead)
            {
                // ランダムに選択
                int randomCount = Random.Range(0, friends.Count);
                targets.Add(friends[randomCount]);
            }
            else
            {
                targets.Add(target);
            }
        }
        // 味方死亡キャラ
        else if (targetType == TargetType.DeadFriendChoose)
        {
            targets.Add(target);
        }
        // 敵ランダム
        else if (targetType == TargetType.OpponentRandom)
        {
            int randomCount = Random.Range(0, opponents.Count);
            targets.Add(opponents[randomCount]);
        }
        // 単体攻撃
        else
        {
            // 対象が既に削除済みの場合
            if (target == null)
            {
                // ランダムに選択
                int randomCount = Random.Range(0, opponents.Count);
                targets.Add(opponents[randomCount]);
            }
            // 対象が戦闘不能
            else if (target.IsDead)
            {
                // ランダムに選択
                int randomCount = Random.Range(0, opponents.Count);
                targets.Add(opponents[randomCount]);
            }
            else
            {
                targets.Add(target);
            }
        }

        return targets;
    }


    // マップの敵キャラとNPCキャラの位置記憶をクリアする
    public static void MapEnemyAndCharacterClear()
    {
        mapEnemiesMemory.Clear();
        mapCharactersMemory.Clear();
    }

}