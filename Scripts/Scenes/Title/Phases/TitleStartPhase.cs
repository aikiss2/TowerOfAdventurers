using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// タイトルシーンの開始フェーズクラス
public class TitleStartPhase : TitlePhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(TitleObjects titleObjects)
    {
        // オプション設定を読み込む
        if (ES3.KeyExists("optionSettingDbSO") == true)
        {
            GlobalCanvasManager.globalCanvasManager.optionSettingDbSO = ES3.Load<OptionSettingDbSO>("optionSettingDbSO");
        }
        else
        {
            ES3.Save<OptionSettingDbSO>("optionSettingDbSO", GlobalCanvasManager.globalCanvasManager.optionSettingDbSO);
        }

        // BGM/SE音量を設定する
        SoundManager.soundManager.SetBgmVolume(GlobalCanvasManager.globalCanvasManager.optionSettingDbSO.bgmVolume / 100.0f);
        SoundManager.soundManager.SetSeVolume(GlobalCanvasManager.globalCanvasManager.optionSettingDbSO.seVolume / 100.0f);

        // BGMを再生する
        SoundManager.soundManager.PlayBgm(TitleManager.titleManager.DefaultBgm);

        // セーブがある場合は「つづきから」を選択状態にする
        if (ES3.KeyExists("playerPartyDbSO") == true)
        {
            titleObjects.selectButtonPanel.Open(1);
        }
        else
        {
            titleObjects.selectButtonPanel.Open();
        }

        // 選択画面に遷移
        next = new TitleChoosePhase();

        yield return null;
    }
}
