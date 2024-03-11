using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Skill/HealSkillSO")]
// 回復スキルクラス
public class HealSkillSO : SkillSO
{
    // 回復レート
    [SerializeField] float healRate;

    // 回復SE
    [SerializeField] SEType seType;

    // 回復量を決める種別
    [SerializeField] VolumeType volumeType;

    // 回復量の基準が「固定値」に参照する固定値
    [SerializeField] int staticValue;

    // スキル終了時のWAIT時間(秒)
    [SerializeField] float waitTime;


    // スキル実行メソッド
    public override IEnumerator Execute(BattleCharacter executer, BattleCharacter target, List<BattleCharacter> friends, List<BattleCharacter> opponents, RawImage effectScreen)
    {
        // ターゲットリストを取得する
        List<BattleCharacter> targets = Utility.GetTargetList(TargetType, target, friends, opponents);

        // MP消費
        executer.SpendTp(SpendTP); 

        int healPoint;

        // 量を決める種別により、回復値を決定する
        if(volumeType == VolumeType.Ability)
        {
            healPoint = (int)(executer.RES * healRate);
            healPoint += Random.Range(0, (int) healPoint / 5);
        }
        else
        {
            healPoint = staticValue;
        }

        // SEを鳴らす
        SoundManager.soundManager.SEPlay(seType);

        // 対象の数だけ回復を実施する
        foreach (BattleCharacter one in targets)
        {
            // 蘇生の場合
            if (TargetType == TargetType.DeadFriendChoose)
            {
                // 対象が戦闘不能の場合、蘇生する
                if (one.IsDead)
                {
                    one.Rebirth();
                    one.Heal(healPoint, true);
                }
                // 対象が戦闘不能でない場合、ミス
                else
                {
                    one.Avoided();
                }
            }
            // 通常回復の場合
            else
            {
                one.Heal(healPoint, true);
            }
        }

        yield return new WaitForSeconds(waitTime);
    }
}
