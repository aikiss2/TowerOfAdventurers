using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

// タイトルシーンの「はじめ」「つづき」「オプション」選択フェーズクラス
public class TitleChoosePhase : TitlePhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(TitleObjects titleObjects)
    {
        // プレイヤーからの入力待ち
        PlayerInput[] playerInput = new PlayerInput[1];
        yield return TitleManager.titleManager.WaitPlayerInputChooseButton(playerInput);
        // 決定
        if (playerInput[0] == PlayerInput.Decide)
        {
            int currentID = titleObjects.selectButtonPanel.CurrentID;

            // はじめから
            if (currentID == 0)
            {
                // セーブがある場合は、セーブを削除するかの確認画面を表示する
                if (ES3.KeyExists("playerPartyDbSO") == true)
                {
                    SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                    // 各種画面の非選択・オープン
                    titleObjects.selectButtonPanel.DeSelect();
                    titleObjects.confirmPanel.Open();
                    titleObjects.yesNoButtonPanel.Open(1);

                    // 確認画面のYES/NO回答待ち
                    yield return TitleManager.titleManager.WaitPlayerInput(playerInput);
                    int id = titleObjects.yesNoButtonPanel.CurrentID;

                    titleObjects.yesNoButtonPanel.Close();
                    titleObjects.confirmPanel.Close();

                    // Yes(セーブを削除し、はじめから開始)
                    if ( (id == 0) && (playerInput[0] == PlayerInput.Decide))
                    {
                        CreateSaveData();
                        yield return StartGameSetting(titleObjects);

                        next = new TitleEndPhase();
                    }
                    // No(選択画面に戻る)
                    else
                    {
                        SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

                        titleObjects.selectButtonPanel.Select();

                        //セーブがあって、データ削除も拒否した場合はタイトル画面のまま
                        next = new TitleChoosePhase();
                    }
                }
                // セーブがない場合は、はじめから開始する
                else
                {
                    CreateSaveData();
                    yield return StartGameSetting(titleObjects);

                    next = new TitleEndPhase();
                }
            }
            // つづきから
            else if (currentID == 1)
            {
                // セーブがある場合はタウンシーンに遷移する。
                if (ES3.KeyExists("playerPartyDbSO") == true)
                {
                    SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                    Utility.nextScene = SceneName.Town;
                    yield return TitleManager.titleManager.Fadeout();
                }
                // セーブがない場合ははじめから開始する。
                else
                {
                    CreateSaveData();
                    yield return StartGameSetting(titleObjects);
                }

                next = new TitleEndPhase();
            }
            // オプション
            else if (currentID == 2)
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                // オプション画面を開き、オプションフェーズに遷移
                titleObjects.selectButtonPanel.DeSelect();
                titleObjects.optionSettingPanel.SetOptionSettingWindow(GlobalCanvasManager.globalCanvasManager.optionSettingDbSO);
                titleObjects.optionSettingPanel.Open();

                next = new TitleOptionSettingPhase();
            }
            else
            {
                next = new TitleChoosePhase();
            }
        }
        // キャンセル
        else if(playerInput[0] == PlayerInput.Cancel)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // デフォルトボタン選択(はじめから)
            titleObjects.selectButtonPanel.ManualSelectButton(0);

            next = new TitleChoosePhase();
        }
        else
        {
            next = new TitleChoosePhase();
        }

        yield return null;
    }

    // セーブデータを作成する
    private void CreateSaveData()
    {
        ES3.Save<PlayerPartyDbSO>("playerPartyDbSO", GlobalCanvasManager.globalCanvasManager.playerPartyDbSO);
        for (int i = 0; i < GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData.Count; i++)
        {
            ES3.Save<PlayerPartyDbDataSO>("PlayerPartyDbData" + i.ToString(), GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[i]);
        }
        ES3.Save<EventMemoryDbSO>("eventMemoryDbSO", GlobalCanvasManager.globalCanvasManager.eventMemoryDbSO);
        ES3.Save<SkillStockDbSO>("skillStockDbSO", GlobalCanvasManager.globalCanvasManager.skillStockDbSO);
        ES3.Save<TalkMemoryDbSO>("talkMemoryDbSO", GlobalCanvasManager.globalCanvasManager.talkMemoryDbSO);
    }

    // 「はじめから」を実行した際のゲーム設定
    private IEnumerator StartGameSetting(TitleObjects titleObjects)
    {
        // 戦闘後のマッププレイヤーの初期位置設定
        Utility.mapPlayerMemory.pos = new Vector3(0, 1.28f, 0);
        Utility.mapPlayerMemory.direction = new Vector2(0, 1);

        // バトルシーンに遷移
        Utility.nextScene = SceneName.Battle;

        // 戦闘開始時BGM再生
        SoundManager.soundManager.PlayBgm(BGMType.BgmNo14);

        // 戦闘開始時のフェードアウト
        yield return DOTween.To(() => titleObjects.battleFade.fade.Range, num => titleObjects.battleFade.fade.Range = num, 1.0f, 1.0f).WaitForCompletion();
    }

}
