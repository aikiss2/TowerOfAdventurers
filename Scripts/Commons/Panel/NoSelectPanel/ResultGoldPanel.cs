using System.Collections;
using TMPro;
using UnityEngine;

public class ResultGoldPanel : MonoBehaviour
{
    // フレームオブジェクト
    [SerializeField] RectTransform frame;

    // 取得ゴールドの値を表示するテキスト
    [SerializeField] TextMeshProUGUI text;

    // フレームをフェードインさせるスピード
    public float speed;


    // 取得ゴールドを設定し、フレームのフェードインを開始する
    private void Start()
    {
        text.text = Utility.getGold.ToString()+"G";
        GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.partyGold += Utility.getGold;
        StartCoroutine(FadeinFrame());
    }


    // フレームをフェードインさせる
    private IEnumerator FadeinFrame()
    {
        while (true)
        {
            var pos = frame.localPosition;
            pos.y -= speed;

            if(pos.y <= 0.0f)
            {
                pos.y = 0;
                frame.localPosition = pos;
                break;
            }

            frame.localPosition = pos;

            yield return null;
        }

        yield break;
    }

}
