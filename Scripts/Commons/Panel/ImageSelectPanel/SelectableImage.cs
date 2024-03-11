using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
// 選択イメージのクラス
public class SelectableImage : Selectable
{
    // プレイヤーにイメージ選択された際に呼び出すメソッド(外部公開用)
    public UnityAction<Transform> OnSelectAction { set { _onSelectAction = value; } }

    // イメージにマウスポインターが入っているかフラグ(外部公開用)
    public bool IsPointerInside
    {
        get { return _isPointerInside; }
    }

    // プレイヤーにイメージ選択された際に呼び出すメソッド
    private UnityAction<Transform> _onSelectAction = null;

    // イメージにマウスポインターが入っているかフラグ(外部公開用)
    private bool _isPointerInside = false;


    // イメージを選択された際、登録されたメソッドを呼び出す
    public override void OnSelect(BaseEventData eventData)
    {
        if (_onSelectAction != null)
        {
            _onSelectAction.Invoke(transform);
        }
    }


    // イメージにマウスポインターが入ってきた際、登録されたメソッドを呼び出す(ポインターが入るだけでも選択とみなす)
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (_onSelectAction != null)
        {
            _onSelectAction.Invoke(transform);
        }

        _isPointerInside = true;
    }


    // イメージにマウスポインターが出て行った際、フラグを落とす
    public override void OnPointerExit(PointerEventData eventData)
    {
        _isPointerInside = false;
    }

}
