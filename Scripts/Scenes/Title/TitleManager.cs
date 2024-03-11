using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

// タイトルシーンのメインループクラス
public class TitleManager : SceneManagerBase
{
    // タイトルシーンのオブジェクト
    [SerializeField] private TitleObjects _titleObjects;

    // デフォルトBGM
    [field: SerializeField] public BGMType DefaultBgm { get; private set; }

    // タイトルシーンの状態
    private TitlePhaseBase _phaseState;

    // タイトルマネージャーを保存するstatic変数
    public static TitleManager titleManager;


    // static変数に自身を保存する
    private void Awake()
    {
        titleManager = this;
    }


    // シーンの初期設定をし、メインループを開始する
    private void Start()
    {
        GlobalCanvasManager.globalCanvasManager.SetCamera(mainCamera);
        Utility.nextScene = SceneName.None;

        _phaseState = new TitleStartPhase();
        StartCoroutine(Main());
    }


    // メインループ。ENDフェーズに遷移したら終了する
    private IEnumerator Main()
    {
        while (_phaseState is not TitleEndPhase)
        {
            yield return _phaseState.Execute(_titleObjects);

            _phaseState = _phaseState.next;
        }

        yield return _phaseState.Execute(_titleObjects);
    }
}


[System.Serializable]
// タイトルシーンのオブジェクト
public struct TitleObjects
{
    // はじめ・続き・オプションの選択パネル
    public SelectButtonPanel selectButtonPanel;

    // オプション設定パネル
    public OptionSettingPanel optionSettingPanel;

    // セーブ削除の確認パネル
    public ModalWindowManager confirmPanel;

    // セーブ削除の回答パネル
    public SelectButtonPanel yesNoButtonPanel;

    // バトル開始のフェードアウト
    public Fade battleFade;
}

