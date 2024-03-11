using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// イベントインターフェース
public interface IEventTask
{
    // イベントを実行する
    public IEnumerator RunEvent(int index, SceneManagerBase sceneManager);
}