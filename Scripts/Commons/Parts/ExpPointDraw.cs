using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// 経験値取得の表示クラス
public class ExpPointDraw : MonoBehaviour
{
    // バーオブジェクト
    [SerializeField] GameObject pointBar;

    // 経験値表示テキスト
    [SerializeField] TextMeshProUGUI expText;

    // プレイヤーのレベルテキスト
    [SerializeField] TextMeshProUGUI levelText;

    // 取得経験値テキスト
    [SerializeField] TextMeshProUGUI getExpText;

    // 経験値取得スピードの下限値
    [SerializeField] int speedMin;

    // 経験値取得スピードの上限値
    [SerializeField] int speedMax;

    // 経験値取得スピードを上げる間隔
    [SerializeField] int speedUpDistance;

    // 本オブジェクトが対応する味方
    private BattlePlayer _battlePlayer;


    // 味方情報を設定する
    public void SetPlayer(BattlePlayer battlePlayer)
    {
        // 各種テキスト情報を設定する
        expText.text = battlePlayer.TotalExp.ToString();
        levelText.text = battlePlayer.Level.ToString();
        
        int max = battlePlayer.playerExpTableSO.playerExpTableData[battlePlayer.Level].NeedExp;
        pointBar.transform.localScale = new Vector3((float)battlePlayer.TotalExp / max, 1.0f, 1.0f);

        getExpText.text = "+" + battlePlayer.GetExp.ToString();

        _battlePlayer = battlePlayer;
    }


    // 経験値取得処理を開始する
    public void StartGetExp()
    {
        // 最高レベルに達していなければ経験値を加算する
        if (_battlePlayer.Level < Define.BATTLE_CHARACTER_LEVEL_MAX)
        {
            StartCoroutine(GetExp(_battlePlayer));
        }
        // 最高レベルの場合は、経験値を無効化する
        else
        {
            _battlePlayer.GetExp = 0;
        }
    }


    // 経験値を徐々に増やし、
    IEnumerator GetExp(BattlePlayer battlePlayer)
    {
        SoundManager.soundManager.SEPlayStopAble(SEType.SeNo27);

        // 増やす量に応じて、ポイントアップの早さを調整する
        int speed = Math.Clamp(battlePlayer.GetExp / speedUpDistance, speedMin, speedMax);

        // レベルアップに必要な経験値を取得
        int max = battlePlayer.playerExpTableSO.playerExpTableData[battlePlayer.Level].NeedExp;

        while (battlePlayer.GetExp != 0)
        {
            // 取得経験値以上にならないようにスピードを抑止する
            if( (battlePlayer.GetExp - speed) < 0)
            {
                speed = battlePlayer.GetExp;
            }

            // レベルアップに必要な経験値を超える場合は、ぴったりにするようにする
            if( (battlePlayer.TotalExp + speed) > max)
            {
                battlePlayer.GetExp -= (max - battlePlayer.TotalExp);
                battlePlayer.TotalExp = max;
            }
            // 越えなければ普通に加減算する
            else
            {
                battlePlayer.GetExp -= speed;
                battlePlayer.TotalExp += speed;
            }

            // 経験値テキストとバーを更新する
            expText.text = battlePlayer.TotalExp.ToString();
            pointBar.transform.localScale = new Vector3((float)battlePlayer.TotalExp / max, 1.0f, 1.0f);

            yield return null;

            // レベルアップに必要な経験値を満たした場合、レベルアップ処理
            if ( battlePlayer.TotalExp == max)
            {
                // レベルアップSEを鳴らす
                SoundManager.soundManager.SEStop();
                SoundManager.soundManager.SEPlay(SEType.SeNo34);
                yield return new WaitForSeconds(0.5f);

                // レベルアップを実行
                battlePlayer.LevelUp();

                // レベルテキストを更新
                levelText.text = battlePlayer.Level.ToString();

                // 最高レベルに達したら経験値取得処理を終了する
                if (battlePlayer.Level == Define.BATTLE_CHARACTER_LEVEL_MAX)
                {
                    battlePlayer.GetExp = 0;
                    break;
                }

                // 現レベルでの経験値をリセットする
                battlePlayer.TotalExp = 0;

                // 表示を更新する
                expText.text = battlePlayer.TotalExp.ToString();
                max = battlePlayer.playerExpTableSO.playerExpTableData[battlePlayer.Level].NeedExp;
                pointBar.transform.localScale = new Vector3((float)battlePlayer.TotalExp / max, 1.0f, 1.0f);

                SoundManager.soundManager.SEPlayStopAble(SEType.SeNo27);
            }
        }

        // SEをストップする
        SoundManager.soundManager.SEStop();

        yield break;
    }
}
