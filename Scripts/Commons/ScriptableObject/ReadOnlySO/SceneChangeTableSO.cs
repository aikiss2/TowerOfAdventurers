using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
// シーン変更先のテーブル
public class SceneChangeTableSO : ScriptableObject
{
    public List<SceneChangeData> sceneChangeTableData = new ();
}

[System.Serializable]
// シーン変更先のデータ
public class SceneChangeData
{
    // 変更先シーンの名前
    [field: SerializeField] public SceneName NextSceneName { get; private set; }

    // 変更先シーンのオブジェクト番号
    [field: SerializeField] public SceneChangeObjectNumber NextObjectNumber { get; private set; }
}