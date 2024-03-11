using DG.Tweening;
using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;


[RequireComponent(typeof(SelectButtonPanel))]
// 選択可能なボタンを生成するパネルクラス
public class CreateButtonPanel : MonoBehaviour
{
    // 選択中のボタンID(外部公開用)
    public int CurrentID
    {
        get { return selectButtonPanel.CurrentID; }
    }

    // 生成する選択可能なボタンのプレハブ
    [SerializeField] SelectableButton selectableButtonPrefab;

    // 生成するボタンを登録するリスト
    [SerializeField] GameObject list;

    // ウィンドウのスクロールバー
    [SerializeField] Scrollbar scrollbar;

    // 現在のマネージャー(ボタンクリックの検知を通知する)
    [SerializeField] SceneManagerBase nowManager;

    // ウィンドウに何個のボタンが表示されるか
    [SerializeField] float drawCount;

    // スクロールの速度
    [SerializeField] float scrollSpeed;

    // 選択可能のボタンパネルを制御するためのコンポーネント
    [SerializeField] private SelectButtonPanel selectButtonPanel;


    //ウィンドウを開く
    public void Open()
    {
        selectButtonPanel.MoveArrowMethod = MoveArrowTo;
        selectButtonPanel.Open();
    }


    //ウィンドウを閉じる
    public void Close()
    {
        Clear();
        gameObject.SetActive(false);
    }


    //ウィンドウを選択状態にする
    public void Select()
    {
        selectButtonPanel.Select();
    }


    // ウィンドウを選択解除状態にする(ウィンドウは閉じない)
    public void DeSelect()
    {
        selectButtonPanel.DeSelect();
    }


    //ボタンを一掃し、スクロールバーを最上段に戻す
    public void Clear()
    {
        selectButtonPanel.arrow.SetParent(transform);
        foreach (var one in selectButtonPanel.SelectableButtons)
        {
            Destroy(one.gameObject);
        }
        selectButtonPanel.SelectableButtons.Clear();

        scrollbar.value = 1.0f;
    }


    // 一番下の行を選択しているかを返す
    public bool IsLastRow()
    {
        if (CurrentID >= (selectButtonPanel.SelectableButtons.Count - 1))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    // ボタンを生成する
    public void CreateSelectableText(string[] strings)
    {
        List<SelectableButton> selectableButtons = new();

        foreach (var one in strings)
        {
            SelectableButton button = Instantiate(selectableButtonPrefab, list.transform);

            button.interactable = false;
            button.GetComponent<ButtonManager>().Interactable(false);

            button.GetComponent<ButtonManager>().SetText(one);
            button.GetComponent<ButtonManager>().onClick.AddListener(nowManager.ButtonClick);
            selectableButtons.Add(button);
        }

        selectButtonPanel.SelectableButtons = selectableButtons;
    }


    // 選択されたボタンに呼び出され、そのボタンの子に矢印をセットする。
    // 表示範囲を外れそうな場所を選択された場合、スクロールする。
    public void MoveArrowTo(Transform parent)
    {
        selectButtonPanel.arrow.SetParent(parent);
        selectButtonPanel.CurrentID = parent.GetSiblingIndex();

        // スクロールさせる
        PanelScroll();

        if (selectButtonPanel.IsSePlay == true)
        {
            SoundManager.soundManager.SEPlay(SEType.SeNo2);
        }

        Debug.Log($"カーソル移動:{CurrentID}");
    }


    // スクロールさせる
    public void PanelScroll()
    {
        // 1ボタンでのスクロール範囲を算出。スクロールバーは表示個数を除いた範囲内で移動するため、表示個数を差し引く。
        float oneButtonScroll = 1.0f / ((float)selectButtonPanel.SelectableButtons.Count - drawCount);

        float nowValue = scrollbar.value;

        float Upper = Mathf.Clamp(1.0f - (((float)CurrentID - (drawCount - 1.0f)) * oneButtonScroll), 0.0f, 1.0f);
        float Lower = Mathf.Clamp(1.0f - ((float)CurrentID * oneButtonScroll), 0.0f, 1.0f);

        if (nowValue > Upper)
        {
            DOTween.To(() => scrollbar.value, num => scrollbar.value = num, Upper, scrollSpeed);
        }
        else if (nowValue <= Lower)
        {
            DOTween.To(() => scrollbar.value, num => scrollbar.value = num, Lower, scrollSpeed);
        }
    }


    // スクロールバーのRECT領域を取得する
    public RectTransform GetScrollbarRect()
    {
        return scrollbar.GetComponent<RectTransform>();
    }
}
