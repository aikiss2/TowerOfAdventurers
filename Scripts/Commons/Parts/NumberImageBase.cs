using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 数値イメージクラスのベースクラス
public class NumberImageBase : MonoBehaviour
{
    // 0～9のイメージ画像
    [SerializeField] Sprite[] NumberTexture;

    // 1桁の横幅
    [SerializeField] int digitWidth;

    // 自オブジェクトが表示担当する桁(上位から数えて)
    private int _myDigit;

    // 自オブジェクトが表示する数値
    private int _myNumber;


    // 表示する数値を設定する
    // numは表示する全体の数値、digitは自オブジェクトが何桁目かを示す(上位から数えて)
    public void SetNumber(int num, int digit)
    {
        // numの桁数を算出
        int inputNumDigit = GetDigit(num);

        // 自オブジェクトが何桁目(上位から数えて)かを算出
        _myDigit = inputNumDigit - (digit - 1);

        // 自オブジェクトが表示する数値を算出
        _myNumber = GetPointDigit(num, _myDigit);

        // 自オブジェクトの数値画像を設定
        GetComponent<SpriteRenderer>().sprite = NumberTexture[_myNumber];

        var position = gameObject.transform.localPosition;

        // 最上位桁の場合は初期位置を調整する
        if (digit == 1) 
        {
            position.x -= (digitWidth * (inputNumDigit - 1));
        }
        // 最上位桁以外
        else
        {
            // 上位の数字が1の場合は左に詰めて表示する
            if (GetPointDigit(num, _myDigit+1) == 1) 
            {
                position.x += 20;
            }
            // 上位の数字が通常の場合は、普通の幅で表示する
            else
            {
                position.x += 25;
            }

        }
        gameObject.transform.localPosition = position;
    }


    // 最後の桁(1桁目)か返す(true:最後の桁である)
    public bool IsLastDigit()
    {
        return (_myDigit == 1);
    }


    // numのdigit桁目の数値を返す関数
    private int GetPointDigit(int num, int digit)
    {
        return (int)(num / Mathf.Pow(10, digit - 1)) % 10;
    }


    //numの桁数を返す関数
    private int GetDigit(int num)
    {
        return (num == 0) ? 1 : ((int)Mathf.Log10(num) + 1);
    }

}
