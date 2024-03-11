using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ButtonManager))]
// 選択ボタンのクラス
public class SelectableButton : Selectable
{
    // プレイヤーにボタン選択された際に呼び出すメソッド(外部公開用)
    public UnityAction<Transform> OnSelectAction { set { _onSelectAction = value; } }

    // ボタン選択された際にダイアログに表示する文字列(外部公開用)
    public string DialogString { set { _dialogString = value; } get { return _dialogString; } }

    // プレイヤーにボタン選択された際に呼び出すメソッド
    private UnityAction<Transform> _onSelectAction = null;

    // ボタン選択された際にダイアログに表示する文字列
    [SerializeField] private string _dialogString = "";


    // プレイヤーにボタン選択された時に呼び出されるメソッド
    public override void OnSelect(BaseEventData eventData)
    {
        if(_onSelectAction != null)
        {
            // ボタン選択時にダイアログに文字列を表示する
            GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog(_dialogString);

            _onSelectAction.Invoke(transform);
        }
    }


    // 有効/無効を切り替える(true:有効 false:無効)
    public void ActivateOrNotActivate(bool set)
    {
        interactable = set;
        GetComponent<ButtonManager>().Interactable(set);
    }


    // マウスがボタンから外れた時に非選択状態にする(true:有効 false:無効)
    public void PointerExitDeSelectSet(bool set)
    {
        GetComponent<ButtonManager>().PointerExitDeSelectSet(set);
    }

}
