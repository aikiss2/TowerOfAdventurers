using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// スキル取得イベント
public class GetSkillEvent : IEventTask
{
    // スキルを取得する
    public IEnumerator RunEvent(int index, SceneManagerBase sceneManager)
    {
        SkillTable skillTable = GlobalCanvasManager.globalCanvasManager.eventTableSO.eventTableData[index].skill;
        SkillStockDbData findSkill = GlobalCanvasManager.globalCanvasManager.skillStockDbSO.skillStockDbData.Find(item => item.skill.SkillTableNo == skillTable);

        findSkill.stockCondition = StockCondition.NoBuy;

        SoundManager.soundManager.SEPlay(SEType.SeNo36);

        GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog("スキル[" + findSkill.skill.Name + "]を手に入れた");

        yield break;
    }
}
