using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// タウンシーンのフェーズ管理のベースクラス
public abstract class TownPhaseBase
{
    // タウンシーン内の次の遷移フェーズ
    public TownPhaseBase next;

    // 各フェーズの実行メソッド
    public abstract IEnumerator Execute(TownObjects townObjects);
}
