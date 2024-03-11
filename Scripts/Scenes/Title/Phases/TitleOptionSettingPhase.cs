using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

// タイトルシーンのオプション設定フェーズクラス
public class TitleOptionSettingPhase : TitlePhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(TitleObjects titleObjects)
    {
        // プレイヤーからの入力待ち
        TitleManager.titleManager.buttonClick = false;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || (TitleManager.titleManager.buttonClick == true) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow));
        // 決定
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || (TitleManager.titleManager.buttonClick == true))
        {
            int currentID = titleObjects.optionSettingPanel.CurrentID;

            // 保存ボタンであれば設定を保存する
            if (currentID == 6)
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                titleObjects.optionSettingPanel.SaveOptionSettingDb(GlobalCanvasManager.globalCanvasManager.optionSettingDbSO);
                ES3.Save<OptionSettingDbSO>("optionSettingDbSO", GlobalCanvasManager.globalCanvasManager.optionSettingDbSO);

                titleObjects.optionSettingPanel.Close();
                titleObjects.selectButtonPanel.Select();

                // タイトル選択に遷移
                next = new TitleChoosePhase();
            }
            // 保存ボタン以外であれば動作なし
            else
            {
                // 本フェーズを継続
                next = new TitleOptionSettingPhase();
            }
        }
        // 左矢印
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            int currentID = titleObjects.optionSettingPanel.CurrentID;

            // 音量を下げたり・フラグをOFFにする
            // BGM音量
            if (currentID == 0)
            {
                titleObjects.optionSettingPanel.ChangeBgmVolumeValue(-1.0f);
            }
            // SE音量
            else if (currentID == 2)
            {
                titleObjects.optionSettingPanel.ChangeSeVolumeValue(-1.0f);
            }
            // コマンド記憶
            else if (currentID == 4) 
            {
                titleObjects.optionSettingPanel.ChangeCommandMemorySwitch(false);
            }

            // 音量調整を緩やかにするためにWaitを入れる
            yield return new WaitForSeconds(0.03f);

            // 本フェーズを継続
            next = new TitleOptionSettingPhase();
        }
        // 右矢印
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            int currentID = titleObjects.optionSettingPanel.CurrentID;

            // 音量を上げたり・フラグをONにする
            // BGM音量
            if (currentID == 0)
            {
                titleObjects.optionSettingPanel.ChangeBgmVolumeValue(1.0f);
            }
            // SE音量
            else if (currentID == 2)
            {
                titleObjects.optionSettingPanel.ChangeSeVolumeValue(1.0f);
            }
            // コマンド記憶
            else if (currentID == 4) 
            {
                titleObjects.optionSettingPanel.ChangeCommandMemorySwitch(true);
            }

            // 音量調整を緩やかにするためにWaitを入れる
            yield return new WaitForSeconds(0.03f);

            // 本フェーズを継続
            next = new TitleOptionSettingPhase();
        }
        else
        {
            next = new TitleOptionSettingPhase();
        }

        yield return null;
    }

}
