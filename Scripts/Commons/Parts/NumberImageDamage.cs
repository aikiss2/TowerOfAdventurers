using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(NumberImageBase))]
// ダメージ数値クラス(上位の桁から順番に表示させ、徐々に大きく表示する)
public class NumberImageDamage : MonoBehaviour
{
    // 次の桁生成のためのプレハブ
    [SerializeField] NumberImageDamage nextNumberImagePrefab;

    // 表示時間
    [SerializeField] float displayTime;

    // 次の桁表示までのWait時間
    [SerializeField] float nextdigitWait;

    // 数値を大きくするスピード
    [SerializeField] float scaleSpeed;

    // 最終的な数値の大きさ
    [SerializeField] float maxScale;

    // 数値イメージクラスのベースクラス
    [SerializeField] NumberImageBase numberImageBase;


    // 表示する数値を設定する
    // numは表示する全体の数値、digitは自オブジェクトが何桁目かを示す(上位から数えて)
    public void SetNumber(int num, int digit)
    {
        // 表示画像と表示位置の設定を、ベースクラスの委譲する
        numberImageBase.SetNumber(num, digit);

        // まだ表示すべき桁があれば、次の桁オブジェクトを生成
        if (numberImageBase.IsLastDigit() == false) 
        {
            StartCoroutine(NextInstantiate(num, digit));
        }

        //一定時間で消す(小さな桁は遅れて生成されるため、その分表示時間を短くする)
        Destroy(gameObject, displayTime - (nextdigitWait * (digit - 1)));
    }


    // 次の桁を生成する
    private IEnumerator NextInstantiate(int num, int digit)
    {
        yield return new WaitForSeconds(nextdigitWait);

        var nextNumberImage = Instantiate(nextNumberImagePrefab, transform.parent);
        nextNumberImage.SetNumber(num, digit + 1);
    }


    // 徐々に数値表示を大きくする
    private void Update()
    {
        var localScale = gameObject.transform.localScale;

        if(localScale.x < maxScale)
        {
            localScale.x += scaleSpeed;
            localScale.y += scaleSpeed;

            if(localScale.x >= maxScale)
            {
                localScale.x = maxScale;
                localScale.y = maxScale;
            }

            gameObject.transform.localScale = localScale;
        }
    }

}
