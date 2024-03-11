using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// イベント実行クラス
public class EventTask
{
    // 全イベントリスト
    private readonly IEventTask[] eventTaskList;


    // コンストラクタ
    public EventTask()
    {
        // イベントリストを設定する(EventTypeと順番を同期させる)
        eventTaskList = new IEventTask[]
        {
            new GetGoldEvent(),
            new GetSkillEvent(),
            new TalkEvent(),
            new ObjectDestroyEvent(),
            new PlayerAddEvent(),
            new PlayBgmEvent(),
            new PlaySeEvent(),
            new FadeoutEvent(),
            new UI_HideEvent(),
            new GetLicenseEvent(),
            new GameClearEvent(),
        };
    }


    // イベントを実行する
    public IEnumerator RunEvent(EventTableIndex eventIndex, SceneManagerBase sceneManager)
    {
        int indexNum = 0;

        // 未実行イベントであれば、イベントを実行する
        if (NoDoneEventIndexSearch(ref indexNum, eventIndex))
        {
            // 一連のイベントを継続して実行する
            for (; indexNum < GlobalCanvasManager.globalCanvasManager.eventTableSO.eventTableData.Count; indexNum++)
            {
                // 別のイベントになれば終了する
                if (GlobalCanvasManager.globalCanvasManager.eventTableSO.eventTableData[indexNum].index != eventIndex)
                {
                    break;
                }

                EventType eventType = GlobalCanvasManager.globalCanvasManager.eventTableSO.eventTableData[indexNum].eventType;

                // イベントを実行する
                yield return eventTaskList[(int)eventType].RunEvent(indexNum, sceneManager);

                // イベント実行済みにする
                GlobalCanvasManager.globalCanvasManager.eventMemoryDbSO.done[indexNum] = true;
            }
        }

        yield break;
    }


    // 未実行のイベントを検索し、見つけた場合はeventTableDataの番地をindexNumに格納し返す
    public bool NoDoneEventIndexSearch(ref int indexNum, EventTableIndex index)
    {
        bool ret = false; // 見つけられたらtrue
        int num = 0;
        foreach (var one in GlobalCanvasManager.globalCanvasManager.eventTableSO.eventTableData)
        {
            if ((one.index == index) && (GlobalCanvasManager.globalCanvasManager.eventMemoryDbSO.done[num] == false))
            {
                ret = true;
                indexNum = num;
                break;
            }

            num++;
        }

        return ret;
    }
}
