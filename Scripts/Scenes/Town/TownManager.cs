using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// タウンシーンのメインループクラス
public class TownManager : SceneManagerBase
{
    // タウンシーンのオブジェクト
    [SerializeField] private TownObjects _townObjects;

    // デフォルトBGM
    [field: SerializeField] public BGMType DefaultBgm { get; private set; }

    // タウンシーンの状態
    private TownPhaseBase _phaseState;

    // タウンマネージャーを保存するstatic変数
    public static TownManager townManager;


    // static変数に自身を保存する
    private void Awake()
    {
        townManager = this;
    }


    // シーンの初期設定をし、メインループを開始する
    private void Start()
    {
        GlobalCanvasManager.globalCanvasManager.SetCamera(mainCamera);
        Utility.nextScene = SceneName.None;
        Fadein();

        _phaseState = new TownStartPhase();
        StartCoroutine(Main());
    }


    // メインループ。ENDフェーズに遷移したら終了する
    private IEnumerator Main()
    {
        while (_phaseState is not TownEndPhase)
        {
            yield return _phaseState.Execute(_townObjects);

            _phaseState = _phaseState.next;
        }

        yield return _phaseState.Execute(_townObjects);
    }

}


[System.Serializable]
// タウンシーンのオブジェクト
public struct TownObjects
{
    // 出発・宿屋・スキル屋・・・の選択パネル
    public SelectButtonPanel selectButtonPanel;

    // オプション設定パネル
    public OptionSettingPanel optionSettingPanel;

    // キャラ図鑑の名前パネル
    public CharaNamePanel charaNamePanel;

    // キャラ図鑑のスキルパネル
    public SkillPanel charaSkillPanel;

    // キャラ図鑑のキャラ絵パネル
    public CharaImagePanel charaImagePanel;

    // キャラ図鑑のステータスパネル
    public StatusPanel statusPanel;

    // キャラ図鑑のキャラ説明パネル
    public CharaStoryPanel charaStoryPanel;
}

