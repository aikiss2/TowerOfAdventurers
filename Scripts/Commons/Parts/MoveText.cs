using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(Rigidbody2D))]
// 動くテキストクラス(徐々に上に移動しながら表示する)
public class MoveText : MonoBehaviour
{
    // 表示時間
    [SerializeField] float displayTime;

    // 上に押し上げる力(大きいほど速くなる)
    [SerializeField] float upSpeedForce;


    // Y軸方向に移動させ、一定時間で消す
    void Start()
    {
        // Y軸方向に力を加え、徐々に上に移動させる
        Vector3 force = new(0f, upSpeedForce, 0f);
        GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

        // 一定時間で消す
        Destroy(gameObject, displayTime);
    }
}
