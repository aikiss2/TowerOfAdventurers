using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// バトルシーンのフェーズ管理のベースクラス
public abstract class BattlePhaseBase
{
    // バトルシーン内の次の遷移フェーズ
    public BattlePhaseBase next;

    // スキル選択中のプレイヤーインデックス
    public int NowPlayerIndex { set; get; } = 0;

    // 各フェーズの実行メソッド
    public abstract IEnumerator Execute(BattleObjects battleObjects);
}
