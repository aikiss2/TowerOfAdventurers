using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



[CreateAssetMenu(menuName = "Skill/BuffSkillSO")]
// バフ・デバフスキルのクラス
public class BuffSkillSO : SkillSO
{
    // 量を決める種別
    [SerializeField] VolumeType volumeType;

    // 量の基準が「固定値」に参照する固定値
    [SerializeField] float staticValue;

    // 能力変更の対象ステータス
    [SerializeField] StatusType statusType;

    // スキル終了時のWAIT時間(秒)
    [SerializeField] float waitTime;

    // エフェクトのプレハブ
    [SerializeField] AnimationController effectPrefab;


    // スキル実行メソッド
    public override IEnumerator Execute(BattleCharacter executer, BattleCharacter target, List<BattleCharacter> friends, List<BattleCharacter> opponents, RawImage effectScreen)
    {
        // ターゲットリストを取得する
        List<BattleCharacter> targets = Utility.GetTargetList(TargetType, target, friends, opponents);

        // MP消費
        executer.SpendTp(SpendTP); 

        float buffPoint;

        // 現状は固定値のみ
        if(volumeType == VolumeType.StaticValue)
        {
            buffPoint = staticValue;
        }
        else
        {
            buffPoint = staticValue;
        }

        foreach (BattleCharacter one in targets)
        {
            // エフェクトを再生する
            yield return RunEffect(effectPrefab, one, effectScreen);

            // バフ・デバフを変更する
            one.ModifyBuff(statusType, buffPoint);
        }

        yield return new WaitForSeconds(waitTime);
    }
}
