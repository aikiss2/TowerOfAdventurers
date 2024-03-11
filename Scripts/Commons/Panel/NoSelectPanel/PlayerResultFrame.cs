using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerResultFrame : MonoBehaviour
{
    // 味方画像
    [SerializeField] Image playerImage;

    // 味方名称
    [SerializeField] TextMeshProUGUI nameText;

    // 経験値バー
    [SerializeField] ExpPointDraw expPointDraw;

    // フレームインするスピード
    [SerializeField] float speed;


    // 味方情報を設定する
    public void SetPlayer(BattlePlayer battlePlayer)
    {
        playerImage.sprite = battlePlayer.GetComponent<SpriteRenderer>().sprite;
        nameText.text = battlePlayer.Name;

        expPointDraw.SetPlayer(battlePlayer);
    }


    // 経験値取得処理を開始する
    public void StartGetExp()
    {
        expPointDraw.StartGetExp();
    }


    // フレームのフェードインを開始する
    private void Start()
    {
        StartCoroutine(FadeinFrame());
    }


    // フレームをフェードインさせる
    private IEnumerator FadeinFrame()
    {
        RectTransform rect = transform.GetComponent<RectTransform>();

        while (true)
        {
            float x = rect.pivot.x;
            x += speed;

            if (x >= 0.5f)
            {
                x = 0.5f;
                rect.pivot = new Vector2(x, 0.5f);
                break;
            }

            rect.pivot = new Vector2(x, 0.5f);

            yield return null;
        }

        yield break;
    }
}
