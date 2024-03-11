using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 会話イベント
public class TalkEvent : IEventTask
{
    // 会話を表示する
    public IEnumerator RunEvent(int index, SceneManagerBase sceneManager)
    {
        var talkIndex = GlobalCanvasManager.globalCanvasManager.eventTableSO.eventTableData[index].talkIndex;
        yield return GlobalCanvasManager.globalCanvasManager.talkWindow.TypeTalk(talkIndex);
    }
}
