using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// スキル屋シーンのフェーズ管理のベースクラス
public abstract class SukiruyaPhaseBase
{
    // スキル屋シーン内の次の遷移フェーズ
    public SukiruyaPhaseBase next;

    // 返却するプレイヤー
    public BattlePlayer SelectPlayer { set; get; }

    // 習得するスキルのインデックス
    public int SelectSkillIndex { set; get; }

    // 各フェーズの実行メソッド
    public abstract IEnumerator Execute(SukiruyaObjects sukiruyaObjects);
}
