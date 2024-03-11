using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ゴールド取得イベント
public class GetGoldEvent : IEventTask
{
    // ゴールドを取得する
    public IEnumerator RunEvent(int index, SceneManagerBase sceneManager)
    {
        int amount = GlobalCanvasManager.globalCanvasManager.eventTableSO.eventTableData[index].amount;

        GlobalCanvasManager.globalCanvasManager.goldPanel.GetGold(amount);

        GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.partyGold += amount;

        SoundManager.soundManager.SEPlay(SEType.SeNo36);

        GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog(amount.ToString() + "Gを手に入れた");

        yield break;
    }
}
