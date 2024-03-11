using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SelectImagePanel))]
// プレイヤーパネルクラス
public class PlayerPanel : MonoBehaviour
{
    // 選択中のイメージID(外部公開用)
    public int CurrentID
    {
        get { return selectImagePanel.CurrentID; }
    }

    // 各プレイヤーを表示するフレームのプレハブ
    [SerializeField] SelectableImage playerFramePrefab;

    // 選択矢印のプレハブ
    [SerializeField] TextMeshProUGUI arrowTextPrefab;

    // 選択可能のイメージパネルを制御するためのコンポーネント
    [SerializeField] SelectImagePanel selectImagePanel;


    // ゲーム開始時にデータをロードし、各プレイヤーのフレームを生成する
    private void Awake()
    {
        // データをロードする
        GlobalCanvasManager.globalCanvasManager.playerPartyDbSO = ES3.Load<PlayerPartyDbSO>("playerPartyDbSO");
        GlobalCanvasManager.globalCanvasManager.eventMemoryDbSO = ES3.Load<EventMemoryDbSO>("eventMemoryDbSO");
        GlobalCanvasManager.globalCanvasManager.skillStockDbSO = ES3.Load<SkillStockDbSO>("skillStockDbSO");
        GlobalCanvasManager.globalCanvasManager.talkMemoryDbSO = ES3.Load<TalkMemoryDbSO>("talkMemoryDbSO");
        GlobalCanvasManager.globalCanvasManager.optionSettingDbSO = ES3.Load<OptionSettingDbSO>("optionSettingDbSO");

        // 現在の味方の数だけ、フレームを生成する
        for (int i = 0; i < GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData.Count; i++)
        {
            GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[i] = ES3.Load<PlayerPartyDbDataSO>("PlayerPartyDbData"+i.ToString());

            SelectableImage playerFrame = Instantiate(playerFramePrefab, transform);
            selectImagePanel.SelectableImages.Add(playerFrame);
            playerFrame.GetComponent<BattlePlayerFrame>().battlePlayer.SetPosition(i);
            playerFrame.GetComponent<BattlePlayerFrame>().battlePlayer.Initialize();
        }

        // 矢印を生成する
        CreateArrow();

        // BGM/SE音量を設定する
        SoundManager.soundManager.SetBgmVolume(GlobalCanvasManager.globalCanvasManager.optionSettingDbSO.bgmVolume / 100.0f);
        SoundManager.soundManager.SetSeVolume(GlobalCanvasManager.globalCanvasManager.optionSettingDbSO.seVolume / 100.0f);
    }


    // 3人目の味方をパーティに加える
    public void AddPlayer3()
    {
        GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData.Add(GlobalCanvasManager.globalCanvasManager.playerPartyDbData_player3);

        SelectableImage playerFrame = Instantiate(playerFramePrefab, transform);
        selectImagePanel.SelectableImages.Add(playerFrame);
        playerFrame.GetComponent<BattlePlayerFrame>().battlePlayer.SetPosition(2);
        playerFrame.GetComponent<BattlePlayerFrame>().battlePlayer.Initialize();

        GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.storyStage = StoryStage.Dungeon1_Player3Add;
    }


    // 指定した味方をパーティから外す
    public void DestroyPlayer(int index)
    {
        // 矢印を１番目に退避
        selectImagePanel.arrow.SetParent(transform.GetChild(0).gameObject.transform, false);

        var selectableImage = selectImagePanel.SelectableImages[index];
        selectImagePanel.SelectableImages.Remove(selectableImage);
        Destroy(selectableImage.gameObject);
    }


    //ウィンドウを閉じる
    public void Close()
    {
        gameObject.SetActive(false);
    }


    //ウィンドウを開く
    public void Open(bool select = true)
    {
        selectImagePanel.CurrentID = 0;

        gameObject.SetActive(true);

        selectImagePanel.arrow.GetComponent<TextMeshProUGUI>().text = "▼";

        // もしマウスポインターが既に対象を選択している場合は、初期値をその対象に設定する
        int id = 0;
        foreach (var one in selectImagePanel.SelectableImages)
        {
            if (one.IsPointerInside == true)
            {
                selectImagePanel.CurrentID = id;
                break;
            }

            id++;
        }

        // ウィンドウを開く処理を委譲する
        selectImagePanel.Open(selectImagePanel.CurrentID);

        // 非選択状態を指定されていれば、非選択状態にする
        if( select == false )
        {
            DeSelect();
        }
    }


    //ウィンドウを選択状態にする
    public void Select()
    {
        selectImagePanel.arrow.GetComponent<TextMeshProUGUI>().text = "▼";
        selectImagePanel.Select();
    }


    // ウィンドウを非選択状態にする(ウィンドウは閉じない)
    public void DeSelect()
    {
        selectImagePanel.arrow.GetComponent<TextMeshProUGUI>().text = "";

        selectImagePanel.DeSelect();

        selectImagePanel.IsSePlay = false;
    }


    // セーブする
    public void PlayerSave()
    {
        foreach (var one in GetPlayerList())
        {
            one.BuffReset();
            one.SavePlayerStatus();
        }

        ES3.Save<PlayerPartyDbSO>("playerPartyDbSO", GlobalCanvasManager.globalCanvasManager.playerPartyDbSO);
        for (int i = 0; i < GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData.Count; i++)
        {
            ES3.Save<PlayerPartyDbDataSO>("PlayerPartyDbData" + i.ToString(), GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[i]);
        }

        ES3.Save<EventMemoryDbSO>("eventMemoryDbSO", GlobalCanvasManager.globalCanvasManager.eventMemoryDbSO);
        ES3.Save<SkillStockDbSO>("skillStockDbSO", GlobalCanvasManager.globalCanvasManager.skillStockDbSO);
        ES3.Save<TalkMemoryDbSO>("talkMemoryDbSO", GlobalCanvasManager.globalCanvasManager.talkMemoryDbSO);
        ES3.Save<OptionSettingDbSO>("optionSettingDbSO", GlobalCanvasManager.globalCanvasManager.optionSettingDbSO);
    }


    // 現在選択されている味方を取得する
    public BattlePlayer GetSelectPlayer()
    {
        return selectImagePanel.SelectableImages[CurrentID].GetComponent<BattlePlayerFrame>().battlePlayer;
    }


    // パーティに入っている味方のリストを取得する
    public List<BattlePlayer> GetPlayerList()
    {
        List<BattlePlayer> players = new();

        foreach (var one in selectImagePanel.SelectableImages)
        {
            players.Add(one.GetComponent<BattlePlayerFrame>().battlePlayer);
        }

        return players;
    }


    // 生きている味方のリストを取得する
    public List<BattlePlayer> GetAlivePlayerList()
    {
        List<BattlePlayer> players = new();

        foreach (var one in selectImagePanel.SelectableImages)
        {
            var battlePlayer = one.GetComponent<BattlePlayerFrame>().battlePlayer;

            if(battlePlayer.IsDead == false)
            {
                players.Add(battlePlayer);
            }
        }

        return players;
    }


    // 矢印を生成する
    private void CreateArrow()
    {
        // 一番目の子オブジェクトに付加する
        TextMeshProUGUI text = Instantiate(arrowTextPrefab, transform.GetChild(0).gameObject.transform);

        selectImagePanel.arrow = text.rectTransform;
    }
}
