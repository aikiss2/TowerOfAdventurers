using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

// タウンシーンのオプション設定フェーズクラス
public class TownOptionSettingPhase : TownPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(TownObjects townObjects)
    {
        TownManager.townManager.buttonClick = false;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || (TownManager.townManager.buttonClick == true) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow));
        // 決定
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || (TownManager.townManager.buttonClick == true))
        {
            int currentID = townObjects.optionSettingPanel.CurrentID;

            // 保存ボタンであれば設定を保存する
            if (currentID == 6)
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                townObjects.optionSettingPanel.SaveOptionSettingDb(GlobalCanvasManager.globalCanvasManager.optionSettingDbSO);
                ES3.Save<OptionSettingDbSO>("optionSettingDbSO", GlobalCanvasManager.globalCanvasManager.optionSettingDbSO);

                townObjects.optionSettingPanel.Close();
                townObjects.selectButtonPanel.Select();

                // タイトル選択に遷移
                next = new TownChoosePhase();
            }
            // 保存ボタン以外であれば動作なし
            else
            {
                // 本フェーズを継続
                next = new TownOptionSettingPhase();
            }
        }
        // 左矢印
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            int currentID = townObjects.optionSettingPanel.CurrentID;

            // 音量を下げたり・フラグをOFFにする
            // BGM音量
            if (currentID == 0)
            {
                townObjects.optionSettingPanel.ChangeBgmVolumeValue(-1.0f);
            }
            // SE音量
            else if (currentID == 2)
            {
                townObjects.optionSettingPanel.ChangeSeVolumeValue(-1.0f);
            }
            // コマンド記憶
            else if (currentID == 4)
            {
                townObjects.optionSettingPanel.ChangeCommandMemorySwitch(false);
            }

            // 音量調整を緩やかにするためにWaitを入れる
            yield return new WaitForSeconds(0.03f);

            // 本フェーズを継続
            next = new TownOptionSettingPhase();
        }
        // 右矢印
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            int currentID = townObjects.optionSettingPanel.CurrentID;

            // 音量を上げたり・フラグをONにする
            // BGM音量
            if (currentID == 0)
            {
                townObjects.optionSettingPanel.ChangeBgmVolumeValue(1.0f);
            }
            // SE音量
            else if (currentID == 2)
            {
                townObjects.optionSettingPanel.ChangeSeVolumeValue(1.0f);
            }
            // コマンド記憶
            else if (currentID == 4)
            {
                townObjects.optionSettingPanel.ChangeCommandMemorySwitch(true);
            }

            // 音量調整を緩やかにするためにWaitを入れる
            yield return new WaitForSeconds(0.03f);

            // 本フェーズを継続
            next = new TownOptionSettingPhase();
        }
        else
        {
            next = new TownOptionSettingPhase();
        }

        yield return null;
    }

}
