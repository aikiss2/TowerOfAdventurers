using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapMoveObject))]
[RequireComponent(typeof(MapCharaCommonParameter))]
// マップのボスキャラクラス(戦闘に移行するキャラ)
public class MapBossCharaController : MonoBehaviour, IInteract
{
    // 会話イベントのインデックス
    [SerializeField] TalkTableIndex talkTableIndex;

    // 会話中のBGM
    [SerializeField] BGMType talkBgm;

    // 戦闘する敵情報
    [SerializeField] CreateEnemyTableSO createEnemyTable;

    // 戦闘BGM
    [SerializeField] BGMType battleBgm;

    // 戦闘背景
    [SerializeField] Sprite battleBackground;

    // 戦闘終了後のイベント(マップ遷移時にイベント実行)
    [SerializeField] EventTableIndex nextEvent;

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


    // プレイヤーとの交流(会話後に戦闘シーンに移行)
    public IEnumerator Interact(MapPlayerController mapPlayer)
    {
        mapMoveObject.LookToward(mapPlayer.transform.position);

        // BGM指定があればBGMを変更する
        if (talkBgm != BGMType.None)
        {
            SoundManager.soundManager.PlayBgm(talkBgm);
            yield return null; // 音声切り替え後はWaitを入れないと、トーク音が鳴らないため
        }

        // 会話
        yield return GlobalCanvasManager.globalCanvasManager.talkWindow.TypeTalk(talkTableIndex);

        // 戦闘の敵を設定する
        GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.encountEnemyNumbers.Clear();
        for (int i = 0; i < createEnemyTable.createEnemyTableData.Count; i++)
        {
            int createEnemyNumber = createEnemyTable.createEnemyTableData[i].EnemyNumber;
            GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.encountEnemyNumbers.Add(createEnemyNumber);
        }

        // 戦闘シーンの設定を行う
        GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.battleBgm = battleBgm;
        GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.battleBackground = battleBackground; // nullが入っても大丈夫
        mapPlayer.NextSceneNo = Define.DUNGEON_NEXT_BATTLE_SCENE_NO;
        mapPlayer.IsEncount = true;

        // イベントがある場合は遷移させる。
        if (nextEvent != EventTableIndex.None)
        {
            Utility.nextEvent = nextEvent;
        }
    }


    // プレイヤーと交流できるかどうかを返す(常に交流できる)
    public bool GetInteractAble()
    {
        return true;
    }
}
