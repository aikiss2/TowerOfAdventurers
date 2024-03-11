using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// BGM再生イベント
public class PlayBgmEvent : IEventTask
{
    // BGMを再生する
    public IEnumerator RunEvent(int index, SceneManagerBase sceneManager)
    {
        var bgmType = GlobalCanvasManager.globalCanvasManager.eventTableSO.eventTableData[index].bgmType;
        SoundManager.soundManager.PlayBgm(bgmType);

        yield break;
    }
}
