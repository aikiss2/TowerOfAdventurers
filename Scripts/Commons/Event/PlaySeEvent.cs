using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// SE再生イベント
public class PlaySeEvent : IEventTask
{
    // SEを再生する
    public IEnumerator RunEvent(int index, SceneManagerBase sceneManager)
    {
        var seType = GlobalCanvasManager.globalCanvasManager.eventTableSO.eventTableData[index].seType;
        SoundManager.soundManager.SEPlay(seType);

        yield break;
    }
}
