using Michsky.MUIP;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;


// 選択可能なイメージを持つパネル
public sealed class SelectImagePanel : MonoBehaviour
{
    // 選択中のイメージID(外部公開用)
    public int CurrentID
    {
        set { _currentID = value; }
        get { return _currentID; }
    }

    // 選択イメージ群
    public List<SelectableImage> SelectableImages
    {
        set { _selectableImages = value; }
        get { return _selectableImages; }
    }

    // イメージに設定するメソッド
    public UnityAction<Transform> MoveArrowMethod { set; get; }

    // 選択イメージのホバー音が意図しない時に鳴らないように制御するフラグ
    public bool IsSePlay { set; get; }

    // 選択中のイメージを識別するためのオブジェクト(矢印)。選択中のイメージの子にする。
    [SerializeField] public Transform arrow;

    // 選択イメージ群
    [SerializeField] private List<SelectableImage> _selectableImages = new ();

    // 選択中のイメージID
    private int _currentID = 0;


    // コンストラクタ
    // イメージ選択されたときのメソッドを指定する
    SelectImagePanel()
    {
        MoveArrowMethod = MoveArrowTo;
    }


    //ウィンドウを開く(初期選択位置を指定可能)
    public void Open(int id = 0)
    {
        _currentID = id;

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
        arrow.SetParent(transform, false);
        DeSelect();
        gameObject.SetActive(false);
    }


    //ウィンドウを選択状態にする
    public void Select()
    {
        IsSePlay = false;
        ActivateOrNotActivate(true);
        EventSystem.current.SetSelectedGameObject(_selectableImages[_currentID].gameObject);
        IsSePlay = true;
    }


    // ウィンドウを選択解除状態にする(ウィンドウは閉じない)
    public void DeSelect()
    {
        ActivateOrNotActivate(false);
    }


    // 手動でイメージを選択する
    public void ManualSelectImage(int id)
    {
        _currentID = id;
        IsSePlay = false;
        EventSystem.current.SetSelectedGameObject(_selectableImages[_currentID].gameObject);
        IsSePlay = true;
    }


    // 選択イメージを追加する
    public void AddSelectableImage(SelectableImage selectable)
    {
        selectable.OnSelectAction = MoveArrowMethod;
        _selectableImages.Add(selectable);
    }


    // 選択イメージに矢印を制御するメソッドを設定する
    void SetMoveArrowFunction()
    {
        foreach (SelectableImage one in _selectableImages)
        {
            one.OnSelectAction = MoveArrowMethod;
        }
    }


    // 選択されたイメージに呼び出され、そのイメージの子に矢印をセットする。
    // 選択中のイメージIDを更新する
    public void MoveArrowTo(Transform parent)
    {
        arrow.SetParent(parent, false);
        _currentID = parent.GetSiblingIndex();

        if(IsSePlay == true)
        {
            SoundManager.soundManager.SEPlay(SEType.SeNo2);
        }

        Debug.Log($"カーソル移動:{_currentID}");
    }


    // 選択可能なオブジェクトを有効/無効を切り替える(true:有効 false:無効)
    private void ActivateOrNotActivate(bool flag)
    {
        foreach (SelectableImage one in _selectableImages)
        {
            one.interactable = flag;
        }
    }
}
