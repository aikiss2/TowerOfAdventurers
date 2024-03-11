using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// ゲーム中、常時存在させるキャンバス
public class GlobalCanvasManager : MonoBehaviour
{
    // ダイアログ
    public Dialog dialog;

    // 会話ウィンドウ
    public TalkWindow talkWindow;

    // プレイヤーパネル
    public PlayerPanel playerPanel;

    // はい/いいえ回答パネル
    public SelectButtonPanel yesNoButtonPanel;

    // ゴールドパネル
    public GoldPanel goldPanel;

    // エンカウントした敵データベース
    public EncountEnemyDbSO encountEnemyDbSO;

    // 敵キャラクターのステータステーブル
    public EnemyStatusTableSO enemyStatusTableSO;

    // プレイヤーパーティのデータベース
    public PlayerPartyDbSO playerPartyDbSO;

    // スキルストック状態データベース
    public SkillStockDbSO skillStockDbSO;

    // オプション設定データベース
    public OptionSettingDbSO optionSettingDbSO;

    // 3人目の味方のデータベース
    public PlayerPartyDbDataSO playerPartyDbData_player3;

    // イベント実行データベース
    public EventMemoryDbSO eventMemoryDbSO;

    // 会話実行データベース
    public TalkMemoryDbSO talkMemoryDbSO;

    // イベントデータのテーブル
    public EventTableSO eventTableSO;

    // 会話データのテーブル
    public TalkTableSO talkTableSO;

    // サウンドマネージャーを保存するstatic変数
    public static GlobalCanvasManager globalCanvasManager;


    // static変数に自身を保存する(シングルトン)
    private void Awake()
    {
        // ゲーム起動時
        if (globalCanvasManager == null)
        {
            // ゲームのフレームレートを設定
            Application.targetFrameRate = Define.GAME_FRAME_RATE;

            // static変数に自分を保存する
            globalCanvasManager = this;

            // シーンが変わってもゲームオブジェクトを破棄しない
            DontDestroyOnLoad(gameObject);
        }
        // シーン切り替え時
        else
        {
            // シーン先のサウンドマネージャーを破棄する
            Destroy(gameObject);
        }
    }


    // シーンのカメラを設定する
    public void SetCamera(Camera camera)
    {
        GetComponent<Canvas>().worldCamera = camera;
    }

}
