using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapMoveObject))]
[RequireComponent(typeof(MapCharaCommonParameter))]
// マップのNPCキャラクラス
public class MapCharaController : MonoBehaviour, IInteract
{
    // 会話イベントのインデックス
    [SerializeField] TalkTableIndex talkTableIndex;

    // マップの移動可能オブジェクトを制御するためのコンポーネント
    [SerializeField] private MapMoveObject mapMoveObject;


    // 毎フレーム移動判定を行い、移動するタイミングの場合はランダム移動する
    void Update()
    {
        if (mapMoveObject.MoveTimingJudge() == true)
        {
            mapMoveObject.RundomDirectionMove();
        }
    }


    // プレイヤーとの交流(会話する)
    public IEnumerator Interact(MapPlayerController mapPlayer)
    {
        mapMoveObject.LookToward(mapPlayer.transform.position);

        yield return GlobalCanvasManager.globalCanvasManager.talkWindow.TypeTalk(talkTableIndex);
    }


    // プレイヤーと交流できるかどうかを返す(常に交流できる)
    public bool GetInteractAble()
    {
        return true;
    }
}
