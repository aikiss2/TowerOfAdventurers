using System.Collections;
using UnityEngine;

// タウンシーンのキャラ図鑑のスキル選択フェーズクラス
public class TownChooseSkillPhase : TownPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(TownObjects townObjects)
    {
        // プレイヤーからの入力待ち(スクロールバー領域の左クリックは決定として処理しないよう指定)
        PlayerInput[] playerInput = new PlayerInput[1];
        yield return TownManager.townManager.WaitPlayerInputNoClickRect(playerInput, townObjects.charaSkillPanel.GetScrollbarRect());
        // 決定・キャンセル(決定・キェンセルの両方で、キャラ選択フェーズに戻るというキャンセル動作をする)
        if ( (playerInput[0] == PlayerInput.Decide) || (playerInput[0] == PlayerInput.Cancel) )
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // スキルパネルを非選択にし、キャラ名パネルを選択状態にする
            townObjects.charaSkillPanel.DeSelect();
            townObjects.charaNamePanel.Select();

            next = new TownChooseCharaPhase();
        }
        else
        {
            next = new TownChooseSkillPhase();
        }

        yield return null;
    }

}
