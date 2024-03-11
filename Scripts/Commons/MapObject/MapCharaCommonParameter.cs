using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// マップキャラの共通データ
public sealed class MapCharaCommonParameter : MonoBehaviour
{
    // マップキャラを無効にするイベントインデックス(外部公開用)
    public EventTableIndex DestroyEventIndex
    {
        get { return destroyEventIndex; }
    }

    // マップキャラを無効にするイベントインデックス
    [SerializeField] EventTableIndex destroyEventIndex;
}
