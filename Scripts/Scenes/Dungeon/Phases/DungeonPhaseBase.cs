using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ダンジョンシーンのフェーズ管理のベースクラス
public abstract class DungeonPhaseBase
{
    // ダンジョンシーン内の次の遷移フェーズ
    public DungeonPhaseBase next;

    // 各フェーズの実行メソッド
    public abstract IEnumerator Execute(DungeonObjects dungeonObjects);

    // ダンジョン内のキャラクターの移動可否を制御する(dontMoveをtrue:移動不可 false:移動可) 
    public void DungeonCharacterDontMoveSet(bool dontMove, DungeonObjects dungeonObjects)
    {
        dungeonObjects.mapPlayer.IsDontMove = dontMove;
        foreach (var one in dungeonObjects.mapEnemies)
        {
            one.IsDontMove = dontMove;
        }

        foreach (var one in dungeonObjects.mapCharacters)
        {
            one.IsDontMove = dontMove;
        }
    }
}
