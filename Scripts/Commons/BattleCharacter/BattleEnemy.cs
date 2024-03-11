using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 戦闘用の敵クラス
public class BattleEnemy : BattleCharacter
{
    // 自身が倒された時にプレイヤーパーティが取得するゴールド
    [HideInInspector] public int Gold;

    // 自身が倒された時にプレイヤーパーティが取得する経験値
    [HideInInspector] public int Exp;

    // 各スキルの使用確率(全リストの合計が100になること)
    [HideInInspector] public List<int> enemySkillOdds = new ();

    // 敵の種別番号
    private int _enemyNumber;


    // 初期化
    public override void Initialize()
    {
        // 敵タイプを設定
        _BattlerType = BattlerType.Enemy;

        // 敵の種別番号(_enemyNumber)により、データベースから敵情報を取得し、設定する
        _name = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].Name;
        _HP = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].MaxHP;
        _TP = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].MaxTP;
        Exp = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].Exp;

        spriteDrawObject.sprite = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].Texture;

        skills = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].Skills;
        enemySkillOdds = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].EnemySkillOdds;

        _MaxHP = HP;
        _MaxTP = TP;
        _ATK = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].ATK;
        _DEF = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].DEF;
        _INT = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].INT;
        _RES = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].RES;
        _DEX = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].DEX;
        _AGI = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].AGI;
        _FireDef = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].FireDef;//火
        _WaterDef = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].WaterDef;//水
        _WindDef = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].WindDef;//風
        _SoilDef = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].SoilDef;//土
        _ThunderDef = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].ThunderDef;//雷
        Gold = GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[_enemyNumber].DropGold;

        // その他初期化をベースクラスに委譲
        base.Initialize();
    }


    // スキル実行
    public override IEnumerator SkillExecute(List<BattlePlayer> players, List<BattleEnemy> enemies, RawImage effectScreen)
    {
        List<BattleCharacter> playersList = new(players);
        List<BattleCharacter> enemiesList = new(enemies);
        yield return selectSkill.Execute(this, Target, enemiesList, playersList, effectScreen);
    }


    // スキル実行前にイメージを暗くする処理を開始する
    public override IEnumerator SkillStart()
    {
        yield return StartCoroutine(Dark());
    }


    // イメージを暗くする
    private IEnumerator Dark()
    {
        SpriteRenderer sprite = spriteDrawObject;
        int color = 255;

        while (true)
        {
            color -= 12;

            if (color <= 100)
            {
                color = 100;
                sprite.color = new Color32((byte)color, (byte)color, (byte)color, 255);
                break;
            }

            sprite.color = new Color32((byte)color, (byte)color, (byte)color, 255);


            yield return null;
        }

        yield break;
    }


    // スキル実行後に暗くしたイメージを元に戻す
    public override IEnumerator SkillEnd()
    {
        yield return StartCoroutine(Bright());
    }


    // イメージを明るくする
    IEnumerator Bright()
    {
        SpriteRenderer sprite = spriteDrawObject;
        int color = 100;

        while (true)
        {
            color += 12;

            if (color >= 255)
            {
                color = 255;
                sprite.color = new Color32((byte)color, (byte)color, (byte)color, 255);
                break;
            }

            sprite.color = new Color32((byte)color, (byte)color, (byte)color, 255);

            yield return null;
        }

        yield break;
    }


    // ダメージ演出を開始する
    public override void Damage(int damagePoint)
    {
        StartCoroutine(DamageAndDying(damagePoint));
    }


    // ダメージおよび戦闘不能の処理を実行する
    private IEnumerator DamageAndDying(int damagePoint)
    {
        _IsExecuting = true;

        _HP = Math.Clamp(HP - damagePoint, Define.HP_MIN_LIMIT, Define.HP_MAX_LIMIT);

        yield return StartCoroutine(DamageCoroutine(damagePoint));

        // 戦闘不能の場合、演出終了フラグはOFFにせず、オブジェクト削除されるまでフラグONを継続させる
        if (HP == 0)
        {
            StartDying();
            yield return StartCoroutine(DyingCoroutine());
            _IsDead = true;
        }
        // 生存の場合、演出を終了する
        else
        {
            _IsExecuting = false;
        }
    }


    // 戦闘不能時の処理を実行する
    private void StartDying()
    {
        BuffReset();

        SoundManager.soundManager.SEPlay(SEType.SeNo24);
    }


    // 戦闘不能の演出を実行する
    private IEnumerator DyingCoroutine()
    {
        float _totalDyingTime = 0.0f;
        float DyingDuration = 0.5f;

        // 敵が倒されたときは、徐々に薄くする
        while (true)
        {
            float alpha = 255f;
            _totalDyingTime += Time.deltaTime;
            var ratio = 1.0f - _totalDyingTime / DyingDuration;
            alpha *= ratio; // フェードアウトさせるため、経過時間により揺れの量を減衰

            if (alpha < 0)
            {
                alpha = 0;
            }

            SpriteRenderer sprite = spriteDrawObject;
            sprite.color = new Color32(255, 255, 255, (byte)alpha);

            if (_totalDyingTime >= DyingDuration)
            {
                yield return new WaitForSeconds(0.1f); // 倒したときの余韻
                break;
            }

            yield return null;
        }

        yield break;
    }


    // 回復演出を開始する
    public override void Heal(int healPoint, bool effect)
    {
        StartCoroutine(HealCoroutine(healPoint, false));
    }


    // TP消費メソッド(敵はTP変化しない)
    public override void SpendTp(int spendPoint)
    {
        return;
    }


    // TP回復メソッド(敵はTP変化しない)
    public override void HealTp(int healPoint)
    {
        return;
    }


    // HPを初期化する(TP処理は無し)
    public override void InitializeHpTp()
    {
        _HP = MaxHP;
        hpDrawObject.InitializePoint(HP, MaxHP);
    }


    // 敵の種別番号をセットする
    public void SetEnemyNumber(int number)
    {
        _enemyNumber = number;
    }
}
