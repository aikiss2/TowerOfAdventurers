using DG.Tweening;
using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;


[RequireComponent(typeof(SelectButtonPanel))]
[RequireComponent(typeof(CreateButtonPanel))]
// キャラ図鑑のキャラ選択パネル
public class CharaNamePanel : MonoBehaviour
{
    // 選択中のボタンID(外部公開用)
    public int CurrentID
    {
        get { return selectButtonPanel.CurrentID;}
    }

    // 選択されているキャラの所持スキル表示パネル
    [SerializeField] private SkillPanel charaSkillPanel;

    // 選択されているキャラのイメージパネル
    [SerializeField] private CharaImagePanel charaImagePanel;

    // 選択されているキャラのステータスパネル
    [SerializeField] private StatusPanel statusPanel;

    // 選択されているキャラの説明パネル
    [SerializeField] private CharaStoryPanel charaStoryPanel;

    // 選択可能のボタンパネルを制御するためのコンポーネント
    [SerializeField] private SelectButtonPanel selectButtonPanel;

    // 作成可能なボタンパネルを制御するためのコンポーネント
    [SerializeField] private CreateButtonPanel createButtonPanel;


    //ウィンドウを開く
    public void Open()
    {
        selectButtonPanel.MoveArrowMethod = MoveArrowTo;
        selectButtonPanel.Open();
    }


    //ウィンドウを閉じる
    public void Close()
    {
        createButtonPanel.Close();
    }


    //ウィンドウを選択状態にする
    public void Select()
    {
        createButtonPanel.Select();
    }


    // ウィンドウを選択解除状態にする(ウィンドウは閉じない)
    public void DeSelect()
    {
        createButtonPanel.DeSelect();
    }


    // 一番下の行を選択しているかを返す
    public bool IsLastRow()
    {
        return createButtonPanel.IsLastRow();
    }


    // ボタンを生成する
    public void CreateSelectableText(string[] strings)
    {
        createButtonPanel.CreateSelectableText(strings);
    }


    // 選択されたボタンに呼び出され、そのボタンの子に矢印をセットする。
    // 表示範囲を外れそうな場所を選択された場合、スクロールする。
    // 選択されたキャラのステータス表示する。
    public void MoveArrowTo(Transform parent)
    {
        selectButtonPanel.arrow.SetParent(parent);
        selectButtonPanel.CurrentID = parent.GetSiblingIndex();

        // スクロールさせる
        createButtonPanel.PanelScroll();

        int playerCount = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData.Count;
        // プレイヤーを表示
        if (CurrentID < playerCount)
        {
            charaImagePanel.SetCharaImage(GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[CurrentID].FullBodyTexture);
            statusPanel.SetPlayerStatus(GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[CurrentID]);

            charaSkillPanel.Clear();
            var players = GlobalCanvasManager.globalCanvasManager.playerPanel.GetPlayerList();
            charaSkillPanel.CreateSelectableText(players[CurrentID].skills);

            charaStoryPanel.SetText(GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[CurrentID].story);
        }
        // 最下段のキャンセルボタン以外は、敵キャラクターを表示
        else if (IsLastRow() == false)
        {
            int enemySelect = CurrentID - playerCount;

            // 出会ってる敵のみ情報表示
            if (GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.encountEnemyList[enemySelect])
            {
                charaImagePanel.SetCharaImage(GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[enemySelect].Texture);
                statusPanel.SetEnemyStatus(GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[enemySelect]);

                charaSkillPanel.Clear();
                charaSkillPanel.CreateSelectableText(GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[enemySelect].Skills);

                charaStoryPanel.SetText(GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData[enemySelect].Story);
            }
        }

        if (selectButtonPanel.IsSePlay == true)
        {
            SoundManager.soundManager.SEPlay(SEType.SeNo2);
        }

        Debug.Log($"カーソル移動:{CurrentID}");
    }


    // スクロールバーのRECT領域を取得する
    public RectTransform GetScrollbarRect()
    {
        return createButtonPanel.GetScrollbarRect();
    }
}
