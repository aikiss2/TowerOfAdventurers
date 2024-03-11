using System;
using System.Collections;
using TMPro;
using UnityEngine;

// バー付きの数字(ポイント)表示クラス
public class PointDraw : MonoBehaviour
{
    // バーオブジェクト
    [SerializeField] GameObject pointBar;

    // ポイント表示テキスト
    [SerializeField] TextMeshProUGUI pointText;

    // ポイントアップのスピードの下限値
    [SerializeField] int speedMin;

    // ポイントアップのスピードの上限値
    [SerializeField] int speedMax;

    // ポイントアップのスピードを上げる間隔
    [SerializeField] int speedUpDistance;

    // ポイントの最大値
    private int _nowMaxPoint = 0;

    // ポイントの現在値
    private int _nowPoint = 0;


    // 初期化
    public void InitializePoint(int point, int maxPoint)
    {
        _nowPoint = point;
        _nowMaxPoint = maxPoint;
        pointText.text = point.ToString();
        pointBar.transform.localScale = new Vector3((float)point / maxPoint, 1.0f, 1.0f);
    }


    // 現在のポイントと最大ポイントを設定し、effectにより徐々にバーを伸ばすかどうかを指定する(true:徐々に伸ばす)
    public IEnumerator SetPoint(int point, int maxPoint, bool effect = false)
    {
        // 前回と値が異なる場合は更新
        if ( (_nowPoint != point) || (_nowMaxPoint != maxPoint) )
        {
            _nowMaxPoint = maxPoint;

            // エフェクト指定がtrueでポイントアップの場合は、音を出しながら徐々にバーと数値をあげる
            if ( (point > _nowPoint) && (effect == true) )
            {
                _nowPoint = point;
                yield return StartCoroutine(IncreasePoint());
            }
            // 上記以外は即時にバーと数値を更新する
            else
            {
                _nowPoint = point;
                pointText.text = point.ToString();
                pointBar.transform.localScale = new Vector3((float)point / maxPoint, 1.0f, 1.0f);
            }
        }

        yield return null;
    }


    // ポイントを徐々に増やす
    private IEnumerator IncreasePoint()
    {
        yield return new WaitForSeconds(0.5f);

        // 増やすときのSEを鳴らす(増やし終わるまで)
        SoundManager.soundManager.SEPlayStopAble(SEType.SeNo27);

        int nowDrawPoint = Convert.ToInt32(pointText.text);
        int increasePoint = _nowMaxPoint - _nowPoint;

        // 増やす量に応じて、ポイントアップの早さを調整する
        int speed = Math.Clamp(increasePoint / speedUpDistance, speedMin, speedMax);

        while (true)
        {
            // Max値を超えようとした場合は、スピードを抑止する
            if ( (nowDrawPoint + speed) > _nowMaxPoint)
            {
                speed = _nowMaxPoint - nowDrawPoint;
            }

            // speed分を加算し、表示更新する
            nowDrawPoint += speed;
            pointText.text = nowDrawPoint.ToString();
            pointBar.transform.localScale = new Vector3((float)nowDrawPoint / _nowMaxPoint, 1.0f, 1.0f);

            // もし目標値までポイントアップが済んだら、ループを抜ける
            if( (nowDrawPoint >= _nowMaxPoint) || (nowDrawPoint >= _nowPoint))
            {
                break;
            }

            yield return null;
        }

        // SEを止める
        SoundManager.soundManager.SEStop();

        yield return null;
    }
}
