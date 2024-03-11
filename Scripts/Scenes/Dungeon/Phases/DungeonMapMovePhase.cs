using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// ダンジョンシーンのマップ移動フェーズクラス
public class DungeonMapMovePhase : DungeonPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(DungeonObjects dungeonObjects)
    {
        // プレイヤーからの入力待ち　および　敵エンカウント・シーン移動・マップエネミー生成
        DungeonManager.dungeonManager.buttonClick = false;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0) 
                                         || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetMouseButtonDown(1) 
                                         || DungeonManager.dungeonManager.buttonClick == true  || dungeonObjects.mapPlayer.IsEncount || dungeonObjects.mapPlayer.IsCreateEnemy || dungeonObjects.mapPlayer.IsSceneChange);

        // メニューボタン押下またはキャンセル
        if (DungeonManager.dungeonManager.buttonClick == true || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetMouseButtonDown(1))
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

            // マップのキャラクターを動かなくする
            DungeonCharacterDontMoveSet(true, dungeonObjects);

            // メニューを開く
            OpenMenu(dungeonObjects);

            next = new DungeonChoosePhase();
        }
        // 決定
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0) )
        {
            // プレイヤーアクションが実行できる場合は、アクションを実行する
            if (dungeonObjects.mapPlayer.PlayerActionAble() == true)
            {
                // マップのキャラクターを動かなくする
                DungeonCharacterDontMoveSet(true, dungeonObjects);

                // メニューボタンを消す
                dungeonObjects.openMenuButton.ActivateOrNotActivate(false);

                // プレイヤーアクションを実行する
                yield return dungeonObjects.mapPlayer.PlayerAction();

                // メニューボタンを表示する
                dungeonObjects.openMenuButton.ActivateOrNotActivate(true);

                // マップのキャラクターを動けるようにする
                DungeonCharacterDontMoveSet(false, dungeonObjects);

                next = new DungeonMapMovePhase();
            }
            // アクション実施しない左クリックの場合、プレイヤーは移動を意図しているため何もしない
            else if (Input.GetMouseButtonDown(0))
            {
                next = new DungeonMapMovePhase();
            }
            // アクション実施しない決定ボタン入力の場合はメニュー表示
            else
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                // マップのキャラクターを動かなくする
                DungeonCharacterDontMoveSet(true, dungeonObjects);

                // メニューを開く
                OpenMenu(dungeonObjects);

                next = new DungeonChoosePhase();
            }
        }
        // 敵とのエンカウント
        else if (dungeonObjects.mapPlayer.IsEncount)
        {
            // マップのキャラクターを動かなくする
            DungeonCharacterDontMoveSet(true, dungeonObjects);

            // エンカウントBGMを再生
            SoundManager.soundManager.PlayBgm(BGMType.BgmNo14, false);

            // 戦闘開始時のフェードアウト
            yield return DOTween.To(() => dungeonObjects.battleFade.fade.Range, num => dungeonObjects.battleFade.fade.Range = num, 1.0f, 1.0f).WaitForCompletion();

            next = new DungeonEndPhase();
        }
        // シーン移動
        else if (dungeonObjects.mapPlayer.IsSceneChange)
        {
            // マップのキャラクターを動かなくする
            DungeonCharacterDontMoveSet(true, dungeonObjects);

            // シーン移動SEを鳴らす
            SoundManager.soundManager.SEPlay(SEType.SeNo37);

            // フェードアウト
            yield return DungeonManager.dungeonManager.Fadeout();

            next = new DungeonEndPhase();
        }
        // マップエネミー生成
        else if (dungeonObjects.mapPlayer.IsCreateEnemy)
        {
            dungeonObjects.mapPlayer.IsCreateEnemy = false;

            // マップエネミー生成
            DungeonManager.dungeonManager.CreateEnemy();

            next = new DungeonMapMovePhase();
        }
        else
        {
            next = new DungeonMapMovePhase();
        }

        yield return null;
    }


    // メニューを開く
    private void OpenMenu(DungeonObjects dungeonObjects)
    {
        // メニューボタンを非表示
        dungeonObjects.openMenuButton.gameObject.SetActive(false);

        // 各種パネルを開く
        GlobalCanvasManager.globalCanvasManager.dialog.Open();
        GlobalCanvasManager.globalCanvasManager.goldPanel.Open();
        dungeonObjects.selectButtonPanel.Open();
    }
}
