using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

// スキル屋シーンのメインループクラス
public class SukiruyaManager : SceneManagerBase
{
    // スキル屋シーンのオブジェクト
    [SerializeField] SukiruyaObjects _sukiruyaObjects;

    // デフォルトBGM
    [field: SerializeField] public BGMType DefaultBgm { get; private set; }

    // スキル屋シーンの状態
    SukiruyaPhaseBase _phaseState;

    // スキル屋マネージャーを保存するstatic変数
    public static SukiruyaManager sukiruyaManager;


    // static変数に自身を保存する
    private void Awake()
    {
        sukiruyaManager = this;
    }


    // シーンの初期設定をし、メインループを開始する
    private void Start()
    {
        GlobalCanvasManager.globalCanvasManager.SetCamera(mainCamera);
        Utility.nextScene = SceneName.None;
        Fadein();

        _phaseState = new SukiruyaStartPhase();
        StartCoroutine(Main());
    }


    // メインループ。ENDフェーズに遷移したら終了する
    private IEnumerator Main()
    {
        while (_phaseState is not SukiruyaEndPhase)
        {
            yield return _phaseState.Execute(_sukiruyaObjects);

            _phaseState = _phaseState.next;
        }

        yield return _phaseState.Execute(_sukiruyaObjects);
    }

}


[System.Serializable]
// スキル屋シーンのオブジェクト
public struct SukiruyaObjects
{
    // 購入・習得・返却・・・の選択パネル
    public SelectButtonPanel selectButtonPanel;

    // 購入時のスキル表示パネル
    public SkillPanel buySkillPanel;

    // 習得・返却時のスキル表示パネル
    public SkillPanel skillPanel;
}

