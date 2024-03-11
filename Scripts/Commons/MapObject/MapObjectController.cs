using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR;

// マップのオブジェクトクラス
public class MapObjectController : MonoBehaviour, IInteract
{
    // プレイヤーがアクションした際のイベント
    [SerializeField] EventTableIndex eventTableIndex;

    // イベント実行済みのイメージ(開いた宝箱等)
    [SerializeField] Sprite doneTexture;

    // イベント実行済みフラグ
    private bool _eventDone = false;


    // シーン開始時にイベント実行済みかを判定し、実行済み かつ 実行済みイメージがあればイメージを変更する
    void Start()
    {
        // イベント実行状態を取得し、反映させる
        EventTask eventTask = new ();

        int indexNum = 0;

        // 未実行のイベント検索をし、見つからなければイベント実行済みとして処理する
        if (eventTask.NoDoneEventIndexSearch(ref indexNum, eventTableIndex) == false)
        {
            _eventDone = true;

            if (doneTexture != null)
            {
                // 開いた宝箱等にする
                GetComponent<SpriteRenderer>().sprite = doneTexture;
            }
        }
    }


    // プレイヤーとの交流
    public IEnumerator Interact(MapPlayerController mapPlayer)
    {
        // イベント未実行の場合はイベント実行する
        if(_eventDone == false)
        {
            // ダイアログパネルとゴールドパネルを開く
            GlobalCanvasManager.globalCanvasManager.dialog.Open();
            GlobalCanvasManager.globalCanvasManager.goldPanel.Open();

            // イベント実行
            EventTask eventTask = new();
            yield return eventTask.RunEvent(eventTableIndex, DungeonManager.dungeonManager);

            _eventDone = true;

            if (doneTexture != null)
            {
                // 開いた宝箱等にする
                GetComponent<SpriteRenderer>().sprite = doneTexture; 
            }

            // プレイヤーの入力待ち
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1));

            // ダイアログパネルとゴールドパネルを閉じる
            GlobalCanvasManager.globalCanvasManager.dialog.Close();
            GlobalCanvasManager.globalCanvasManager.goldPanel.Close();
        }

        yield break;
    }


    // プレイヤーと交流できるかどうかを返す(true:交流できる)
    public bool GetInteractAble()
    {
        if(_eventDone == true)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
