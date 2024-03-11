using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 宿屋シーンのフェーズ管理のベースクラス
public abstract class YadoyaPhaseBase
{
    // 宿屋シーン内の次の遷移フェーズ
    public YadoyaPhaseBase next;

    // 各フェーズの実行メソッド
    public abstract IEnumerator Execute(YadoyaObjects yadoyaObjects);
}
