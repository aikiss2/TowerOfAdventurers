using Michsky.MUIP;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

// 選択可能なボタンを持つパネルクラス
public sealed class SelectButtonPanel : MonoBehaviour
{
    // 選択中のボタンID
    public int CurrentID { set; get; } = 0;

    // 選択ボタン群
    public List<SelectableButton> SelectableButtons
    {
        set { _selectableButtons = value; }
        get { return _selectableButtons; }
    }

    // ボタンに設定するメソッド
    public UnityAction<Transform> MoveArrowMethod { set; get; }

    // 選択ボタンのホバー音が意図しない時に鳴らないように制御するフラグ
    public bool IsSePlay { set; get; }

    // 選択中のボタンを識別するためのオブジェクト(矢印)。選択中のボタンの子にする。
    public Transform arrow;

    // 選択ボタン群
    [SerializeField] private List<SelectableButton> _selectableButtons = new();


    // コンストラクタ
    // ボタン選択されたときのメソッドを指定する
    SelectButtonPanel()
    {
        MoveArrowMethod = MoveArrowTo;
    }


    //ウィンドウを開く(初期選択位置を指定可能)
    public void Open(int id = 0)
    {
        CurrentID = id;

        // 開いたときは音を鳴らさない
        IsSePlay = false;

        SetMoveArrowFunction();
        gameObject.SetActive(true);
        Select();

        IsSePlay = true;
    }


    //ウィンドウを閉じる
    public void Close()
    {
        // 矢印をパネルに退避させる
        arrow.SetParent(transform);
        DeSelect();
        gameObject.SetActive(false);
    }


    //ウィンドウを選択状態にする
    public void Select()
    {
        IsSePlay = false;
        ActivateOrNotActivate(true);
        EventSystem.current.SetSelectedGameObject(_selectableButtons[CurrentID].gameObject);
        IsSePlay = true;
    }


    // ウィンドウを選択解除状態にする(ウィンドウは閉じない)
    public void DeSelect()
    {
        ActivateOrNotActivate(false);
    }


    // 手動でボタンを選択する
    public void ManualSelectButton(int id)
    {
        CurrentID = id;
        IsSePlay = false;
        EventSystem.current.SetSelectedGameObject(_selectableButtons[CurrentID].gameObject);
        IsSePlay = true;
    }


    // 選択されたボタンに呼び出され、そのボタンの子に矢印をセットする。
    // 選択中のボタンIDを更新する
    public void MoveArrowTo(Transform parent)
    {
        arrow.SetParent(parent);
        CurrentID = parent.GetSiblingIndex();

        if(IsSePlay == true)
        {
            SoundManager.soundManager.SEPlay(SEType.SeNo2);
        }

        Debug.Log($"カーソル移動:{CurrentID}");
    }


    // 選択ボタンに矢印を制御するメソッドを設定する
    private void SetMoveArrowFunction()
    {
        foreach (var one in _selectableButtons)
        {
            one.OnSelectAction = MoveArrowMethod;
        }
    }


    // 選択可能なオブジェクトを有効/無効を切り替える(true:有効 false:無効)
    private void ActivateOrNotActivate(bool flag)
    {
        foreach (var one in _selectableButtons)
        {
            one.ActivateOrNotActivate(flag);
        }
    }
}
