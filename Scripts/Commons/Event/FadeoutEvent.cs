using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// フェードアウトイベント
public class FadeoutEvent : IEventTask
{
    // フェードアウトを実行する
    public IEnumerator RunEvent(int index, SceneManagerBase sceneManager)
    {
        yield return sceneManager.Fadeout();
    }
}
