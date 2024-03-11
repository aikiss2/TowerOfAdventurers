using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(NumberImageBase))]
[RequireComponent(typeof(Rigidbody2D))]
// 回復数値クラス(徐々に上に移動しながら表示する)
public class NumberImageHeal : MonoBehaviour
{
    // 次の桁生成のためのプレハブ
    [SerializeField] NumberImageHeal nextNumberImagePrefab;

    // 表示時間
    [SerializeField] float displayTime;

    // 上に押し上げる力(大きいほど速くなる)
    [SerializeField] float upSpeedForce;

    // 数値イメージクラスのベースクラス
    [SerializeField] NumberImageBase numberImageBase;


    // 表示する数値を設定する
    // numは表示する全体の数値、digitは自オブジェクトが何桁目かを示す(上位から数えて)
    public void SetNumber(int num, int digit)
    {
        // 表示画像と表示位置の設定を、ベースクラスの委譲する
        numberImageBase.SetNumber(num, digit);

        // Y軸方向に力を加え、徐々に上に移動させる
        Vector3 force = new(0f, upSpeedForce, 0f);
        GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

        // まだ表示すべき桁があれば、次の桁オブジェクトを生成
        if (numberImageBase.IsLastDigit() == false)
        {
            StartCoroutine(NextInstantiate(num, digit));
        }

        //一定時間で消す
        Destroy(gameObject, displayTime);
    }


    // 次の桁を生成する
    private IEnumerator NextInstantiate(int num, int digit)
    {
        var nextNumberImage = Instantiate(nextNumberImagePrefab, transform.parent);
        nextNumberImage.SetNumber(num, digit + 1);

        yield return null;
    }

}
