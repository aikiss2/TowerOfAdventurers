using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 各種スキルクラスのベースクラス
public abstract class SkillSO : ScriptableObject
{
    // スキル識別子
    [field: SerializeField] public SkillTable SkillTableNo { get; private set; }

    // スキル名
    [field: SerializeField] public string Name { get; private set; }

    // スキル使用TP
    [field: SerializeField] public int SpendTP { get; private set; }

    // スキル値段
    [field: SerializeField] public int SpendGold { get; private set; }

    // スキル説明
    [field: SerializeField] public string Explain { get; private set; }

    // スキル対象種別
    [field: SerializeField] public TargetType TargetType { get; private set; }

    // スキル屋で返却できないかフラグ(trueで返却できない)
    [field: SerializeField] public bool IsNoMove { get; private set; }

    // マップで使用できるかフラグ(trueで使用できる)
    [field: SerializeField] public bool IsMapUse { get; private set; }

    // 各スキルクラスの実行メソッド
    public abstract IEnumerator Execute(BattleCharacter executer, BattleCharacter target, List<BattleCharacter> friends, List<BattleCharacter> opponents, RawImage effectScreen);


    // 最終的なダメージ算出
    protected int TotalDamage(int atk, float atkRate, int def, int elementDef)
    {
        // 基本ダメージを算出
        int basicDamage = BasicDamage(atk, def);

        // 基本ダメージからランダムダメージを算出
        int rundomDamage = RundomDamage(basicDamage);

        // 属性防御からダメージ率を算出
        float elementCut = (float)(100 - elementDef) / 100.0f;

        // (基本ダメージ×アタックレート×ダメージ率)＋ランダムダメージ
        int ret = Mathf.Clamp((int)((float)basicDamage * atkRate * elementCut) + rundomDamage, Define.BATTLE_DAMAGE_MIN, Define.BATTLE_DAMAGE_MAX);

        return ret;
    }


    // 基本ダメージ算出
    private int BasicDamage(int atk, int def)
    {
        int ret = (atk / 2) - (def / 4);
        return ret;
    }


    // ランダムダメージ算出
    private int RundomDamage(int damage)
    {
        int range = (damage / 16) + 1;
        int ret = (Random.Range(0, (range * 2) + 1) - range);
        return ret;
    }


    // 攻撃が相手に当たるか判定(当たったらtrueを返す)
    protected bool HitJudge(int dex, int agi)
    {
        bool ret;

        int missRate = Mathf.Clamp((int)((agi - dex) / 10), 1, Define.BATTLE_ATTACK_MISS_RATE_MAX);

        int judgePoint = Random.Range(0, 100);

        // ランダム値がミス確率を下回ったら、ミスSEを再生する
        if(judgePoint < missRate)
        {
            ret = false;
            SoundManager.soundManager.SEPlay(SEType.SeNo33);
        }
        else
        {
            ret = true;
        }

        return ret;
    }


    // 攻撃のクリティカル判定(クリティカルの場合にtrueを返す)
    protected bool CriticalJudge(int dex)
    {
        bool ret;

        int criticalRate = Mathf.Clamp((int)(dex / 80), 1, Define.BATTLE_ATTACK_CRITICAL_RATE_MAX);

        int judgePoint = Random.Range(0, 100);

        // ランダム値がクリティカル確率を下回ったら、クリティカルSEを再生する
        if (judgePoint < criticalRate)
        {
            ret = true;
            SoundManager.soundManager.SEPlay(SEType.SeNo32);
        }
        else
        {
            ret = false;
        }

        return ret;
    }


    // エフェクトを生成し、エフェクト完了まで待つ
    protected IEnumerator RunEffect(AnimationController effectPrefab, BattleCharacter target, RawImage effectScreen)
    {
        AnimationController effect = Instantiate(effectPrefab, target.transform);
        effect.SetEffectObject(effectScreen, target);

        // エフェクト終了まで待つ
        while (effect)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
    }


    // キャラクターのステータスを取得する
    protected int GetStatus(BattleCharacter character, StatusType status)
    {
        int ret = 0;

        switch (status)
        {
            case StatusType.HP:
                ret = character.HP;
                break;
            case StatusType.MaxHP:
                ret = character.MaxHP;
                break;
            case StatusType.TP:
                ret = character.TP;
                break;
            case StatusType.MaxTP:
                ret = character.MaxTP;
                break;
            case StatusType.ATK:
                ret = character.ATK;
                break;
            case StatusType.DEF:
                ret = character.DEF;
                break;
            case StatusType.INT:
                ret = character.INT;
                break;
            case StatusType.RES:
                ret = character.RES;
                break;
            case StatusType.DEX:
                ret = character.DEX;
                break;
            case StatusType.AGI:
                ret = character.AGI;
                break;
        }

        return ret;
    }


    // 攻撃種別に対応するDEFを取得する
    protected int GetDef(BattleCharacter character, AttackType attackType)
    {
        int ret = 0;

        // 物理攻撃
        if (attackType == AttackType.Physical)
        {
            ret = GetStatus(character, StatusType.DEF);
        }
        // 魔法攻撃
        else
        {
            ret = 0;
        }

        return ret;
    }


    // 物理種別に対応する物理属性DEFを取得する
    protected int GetPhysicalDef(BattleCharacter character, PhysicalType physicalType)
    {
        int ret = 0;

        // 物理タイプ
        switch (physicalType)
        {
            case PhysicalType.Cut:
                ret = character.CutDef;
                break;
            case PhysicalType.Thrust:
                ret = character.ThrustDef;
                break;
            case PhysicalType.Hit:
                ret = character.HitDef;
                break;
        }

        return ret;
    }


    // 属性種別に対応する属性DEFを取得する
    protected int GetElementDef(BattleCharacter character, ElementType elementType)
    {
        int ret = 0;

        // 魔法タイプ
        switch (elementType)
        {
            case ElementType.Fire:
                ret = character.FireDef;
                break;
            case ElementType.Water:
                ret = character.WaterDef;
                break;
            case ElementType.Wind:
                ret = character.WindDef;
                break;
            case ElementType.Soil:
                ret = character.SoilDef;
                break;
            case ElementType.Thunder:
                ret = character.ThunderDef;
                break;
        }

        return ret;
    }
}
