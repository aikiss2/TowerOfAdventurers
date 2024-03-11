using System;
using System.Collections;
using System.Reflection;
using System.Xml.Linq;
using TMPro;
using UnityEngine;

// ゴールドパネルクラス
public class GoldPanel : MonoBehaviour
{
    // ゴールドの値を表示するテキスト
    [SerializeField] TextMeshProUGUI text;

    // ゴールド取得スピードの下限値
    [SerializeField] int speedMin;

    // ゴールド取得スピードの上限値
    [SerializeField] int speedMax;

    // ゴールド取得スピードを上げる間隔
    [SerializeField] int speedUpDistance;


    // ゴールドパネルを開く
    public void Open()
    {
        text.text = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.partyGold.ToString() + "G";
        gameObject.SetActive(true);
    }


    // ゴールドパネルを閉じる
    public void Close()
    {
        gameObject.SetActive(false);

        // ゴールド取得SEを念のため停止
        SoundManager.soundManager.SEStop();
    }


    // ゴールド表示を更新
    public void GoldUpdate()
    {
        text.text = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.partyGold.ToString() + "G";
    }


    // ゴールド取得演出を開始する
    public void GetGold(int gold)
    {
        StartCoroutine(getGold(GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.partyGold, gold));
    }


    // ゴールド取得演出を行う
    IEnumerator getGold(int startGold, int getGold)
    {
        yield return new WaitForSeconds(0.3f);

        SoundManager.soundManager.SEPlayStopAble(SEType.SeNo27);

        // 増やす量に応じて、ポイントアップの早さを調整する
        int speed = Math.Clamp(getGold / speedUpDistance, speedMin, speedMax);

        while (getGold != 0)
        {
            // 取得経験値以上にならないようにスピードを抑止する
            if ((getGold - speed) < 0)
            {
                speed = getGold;
            }

            getGold -= speed;
            startGold += speed;

            text.text = startGold.ToString() + "G";

            yield return null;
        }

        SoundManager.soundManager.SEStop();

        yield break;
    }
}
