using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// プレイヤーと交流できるオブジェクトのインターフェース
public interface IInteract
{
    // プレイヤーと交流する
    public IEnumerator Interact(MapPlayerController mapPlayer);

    // プレイヤーと交流できるかを返す
    public bool GetInteractAble();
}