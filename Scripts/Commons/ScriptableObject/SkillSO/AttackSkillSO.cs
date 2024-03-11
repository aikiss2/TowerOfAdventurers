using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Skill/AttackSkillSO")]
// 攻撃スキルクラス
public class AttackSkillSO : SkillSO
{
    // 攻撃レート
    [SerializeField] float atkRate;

    // 攻撃種別
    [SerializeField] AttackType attackType;

    // 攻撃力の依存ステータス
    [SerializeField] StatusType attackStatus;

    // 物理種別
    [SerializeField] PhysicalType physicalType;

    // 属性種別
    [SerializeField] ElementType elementType;

    // 攻撃回数
    [SerializeField] int multiCount;

    // 複数回の攻撃時、攻撃間のWAIT時間(秒)
    [SerializeField] float multiAttackWaitTime;

    // スキル終了時のWAIT時間(秒)
    [SerializeField] float waitTime;

    // エフェクトのプレハブ
    [SerializeField] AnimationController effectPrefab;


    // スキル実行メソッド
    public override IEnumerator Execute(BattleCharacter executer, BattleCharacter target, List<BattleCharacter> friends, List<BattleCharacter> opponents, RawImage effectScreen)
    {
        // 複数攻撃時の攻撃間で倒した相手を管理するためのリストを生成
        List<BattleCharacter> workOpponents = new(opponents);

        // MP消費
        executer.SpendTp(SpendTP);

        // 攻撃回数実行
        for (int i = 0; i < multiCount; i++)
        {
            // ターゲットリストを取得する
            List<BattleCharacter> targets = Utility.GetTargetList(TargetType, target, friends, workOpponents);

            foreach (BattleCharacter one in targets)
            {
                // ターゲットが敵の場合エフェクト表示
                if (one.BattlerType == BattlerType.Enemy)
                {
                    yield return RunEffect(effectPrefab, one, effectScreen);
                }

                // 各種パラメータ取得
                int Atk = GetStatus(executer, attackStatus);
                int Def = GetDef(one, attackType);
                int PhysicalDef = GetPhysicalDef(one, physicalType);
                int ElementDef = GetElementDef(one, elementType);

                // 攻撃実行
                Attack(executer, one, Atk, Def, PhysicalDef, ElementDef);
            }

            // 最後の攻撃でなければ、WAITを入れる
            if(i < (multiCount - 1))
            {
                yield return new WaitForSeconds(multiAttackWaitTime);

                // 演出が終わるまで待つ。相手を倒したかも判定する
                yield return AttackWait(workOpponents);

                // 相手が全て倒したら、攻撃を終了する
                if(workOpponents.Count == 0)
                {
                    break;
                }
            }

        }

        yield return new WaitForSeconds(waitTime);
    }


    // 攻撃を実行する
    private void Attack(BattleCharacter executer, BattleCharacter target, int atk, int def, int physicalDef, int elementDef)
    {
        int damage;

        // 物理攻撃の場合
        if ((attackType == AttackType.Physical))
        {
            // 攻撃が当たるか判定する
            if (HitJudge(executer.DEX, target.AGI))
            {
                // 攻撃する側がプレイヤーの場合、クリティカル判定する
                if (executer.BattlerType == BattlerType.Player)
                {
                    // クリティカルの場合、相手の防御力を0にする
                    if (CriticalJudge(executer.DEX))
                    {
                        def = 0;
                    }
                }

                // ダメージを算出し、相手に与える
                damage = TotalDamage(atk, atkRate, def, elementDef);
                target.Damage(damage);
            }
            // ミスの場合
            else
            {
                // ミス演出を実行
                target.Avoided();
            }
        }
        // 魔法攻撃の場合
        else
        {
            // ダメージを算出し、相手に与える
            damage = TotalDamage(atk, atkRate, def, elementDef);
            target.Damage(damage);
        }
    }


    // 攻撃時の演出(ダメージ・回避・ミス・倒す)の完了を待つ
    private IEnumerator AttackWait(List<BattleCharacter> monitoringTargets)
    {
        // 演出が終わるまで待つ
        while (true)
        {
            bool isExecuting = false;

            // 演出を監視するターゲットを全て精査する
            for (int i = monitoringTargets.Count - 1; i >= 0; i--)
            {
                var one = monitoringTargets[i];

                // 倒した場合は監視対象から外す
                if (one.IsDead == true)
                {
                    monitoringTargets.Remove(one);
                }
                // まだ演出中であれば演出中フラグを立てる
                else if (one.IsExecuting == true)
                {
                    isExecuting = true;
                }
            }

            // 演出中の相手がいなければ終了する
            if (isExecuting == false)
            {
                break;
            }

            yield return null;
        }
    }

}