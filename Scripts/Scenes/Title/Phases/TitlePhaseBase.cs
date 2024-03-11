using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// タイトルシーンのフェーズ管理のベースクラス
public abstract class TitlePhaseBase
{
    // タイトルシーン内の次の遷移フェーズ
    public TitlePhaseBase next;

    // 各フェーズの実行メソッド
    public abstract IEnumerator Execute(TitleObjects titleObjects);
}
