using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

// 宿屋シーンのメインループクラス
public class YadoyaManager : SceneManagerBase
{
    // 宿屋シーンのオブジェクト
    [SerializeField] YadoyaObjects _yadoyaObjects;

    // デフォルトBGM
    [field: SerializeField] public BGMType DefaultBgm { get; private set; }

    // 宿屋１泊の料金
    [field: SerializeField] public int YadoyaSpendGold { get; private set; }

    // 宿屋シーンの状態
    YadoyaPhaseBase _phaseState;

    // 宿屋マネージャーを保存するstatic変数
    public static YadoyaManager yadoyaManager;


    // static変数に自身を保存する
    private void Awake()
    {
        yadoyaManager = this;
    }


    // シーンの初期設定をし、メインループを開始する
    private void Start()
    {
        GlobalCanvasManager.globalCanvasManager.SetCamera(mainCamera);
        Utility.nextScene = SceneName.None;
        Fadein();

        // 前シーンが戦闘の場合は全滅のため、全員生き返らせ、セーブする
        if (Utility.beforeSceneName == SceneName.Battle)
        {
            foreach (var one in GlobalCanvasManager.globalCanvasManager.playerPanel.GetPlayerList())
            {
                if (one.IsDead)
                {
                    one.Rebirth();
                }

                one.InitializeHpTp();
            }

            GlobalCanvasManager.globalCanvasManager.playerPanel.PlayerSave();
        }

        _phaseState = new YadoyaStartPhase();
        StartCoroutine(Main());
    }


    // メインループ。ENDフェーズに遷移したら終了する
    private IEnumerator Main()
    {
        while (_phaseState is not YadoyaEndPhase)
        {
            yield return _phaseState.Execute(_yadoyaObjects);

            _phaseState = _phaseState.next;
        }

        yield return _phaseState.Execute(_yadoyaObjects);
    }

}

[System.Serializable]
// 宿屋シーンのオブジェクト
public struct YadoyaObjects
{
    // 泊まる・会話・戻るの選択パネル
    public SelectButtonPanel selectButtonPanel;
}

