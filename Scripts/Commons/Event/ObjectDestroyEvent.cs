using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// イベントが終了したオブジェクトを無効化するイベント
public class ObjectDestroyEvent : IEventTask
{
    // イベントが終了したオブジェクトを無効化する
    public IEnumerator RunEvent(int index, SceneManagerBase sceneManager)
    {
        EventTableIndex eventIndex = GlobalCanvasManager.globalCanvasManager.eventTableSO.eventTableData[index].index;

        foreach (var one in sceneManager.GetComponent<DungeonManager>().GetDungeonObjects().mapCharacters)
        {
            if (one.GetComponent<MapCharaCommonParameter>().DestroyEventIndex == eventIndex)
            {
                // 無効化する
                one.gameObject.SetActive(false);
            }
        }

        yield break;
    }
}
