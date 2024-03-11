using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// バトルシーンのメインループクラス
public class BattleManager : SceneManagerBase
{
    // バトルシーンのオブジェクト
    [SerializeField] BattleObjects _battleObjects;

    // バトル結果の取得経験値パネル
    [SerializeField] ResultPlayerPanel resultPlayerPanelPrefab;

    // バトル結果の取得ゴールドパネル
    [SerializeField] ResultGoldPanel resultGoldPanelPrefab;

    // バトルシーンの状態
    BattlePhaseBase _phaseState;

    // バトルマネージャーを保存するstatic変数
    public static BattleManager battleManager;


    // static変数に自身を保存する
    private void Awake()
    {
        battleManager = this;
    }


    // シーンの初期設定をし、メインループを開始する
    private void Start()
    {
        GlobalCanvasManager.globalCanvasManager.SetCamera(mainCamera);
        Utility.nextScene = SceneName.None;
        Utility.getPlayersExp = 0;
        Utility.getGold = 0;
        Fadein();

        _phaseState = new BattleStartPhase();
        StartCoroutine(Main());
    }


    // メインループ。ENDフェーズに遷移したら終了する
    private IEnumerator Main()
    {
        while (_phaseState is not BattleEndPhase)
        {
            yield return _phaseState.Execute(_battleObjects);

            _phaseState = _phaseState.next;
        }

        yield return _phaseState.Execute(_battleObjects);
    }


    // バトル結果パネルを生成する
    public void ResultPanelCreate()
    {
        Instantiate(resultGoldPanelPrefab, mainCanvas.transform);
        Instantiate(resultPlayerPanelPrefab, mainCanvas.transform);
    }
}


[System.Serializable]
// バトルシーンのオブジェクト
public struct BattleObjects
{
    // スキルパネル
    public SkillPanel skillPanel;

    // 敵パネル
    public EnemyPanel enemyPanel;

    // バトルエフェクトCanvas
    public RawImage screenEffectCanvas;

    // バトル背景
    public SpriteRenderer battleBackground;
}