using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapMoveObject))]
// ダンジョンマップの敵クラス
public class MapEnemyController : MonoBehaviour
{
    // 移動禁止フラグ(メニュー表示中やイベント中等)
    public bool IsDontMove
    {
        set { mapMoveObject.IsDontMove = value; }
        get { return mapMoveObject.IsDontMove; }
    }

    // 戦闘エンカウントフラグ
    public bool IsEncount
    {
        set { mapMoveObject.IsEncount = value; }
        get { return mapMoveObject.IsEncount; }
    }

    // 該当ダンジョンで戦闘する敵のテーブル
    private CreateEnemyTableSO _createEnemyTable;

    // 該当ダンジョンで戦闘する敵の中での強さ
    public int Power { get; set; }

    // マップの移動可能オブジェクトを制御するためのコンポーネント
    [SerializeField] private MapMoveObject mapMoveObject;


    // 毎フレーム移動判定を行い、移動するタイミングの場合はランダム移動する
    void Update()
    {
        if (mapMoveObject.MoveTimingJudge() == true)
        {
            mapMoveObject.RundomDirectionMove();
        }
    }

    // 戦闘する敵のテーブルを設定する
    public void SetEnemyTable(CreateEnemyTableSO createEnemyTable, int power)
    {
        _createEnemyTable = createEnemyTable;
        Power = power;
    }


    // プレイヤーオブジェクトとの衝突判定
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // プレイヤーと衝突した場合は、エンカウント設定する
        if (collision.gameObject.CompareTag("Player"))
        {
            GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.battleBgm = BGMType.None;
            GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.battleBackground = null;
            GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.encountEnemyNumbers.Clear();

            int enemyCount = Random.Range(1, 4);
            for(int i = 0; i < enemyCount; i++)
            {
                int cnt = _createEnemyTable.createEnemyTableData.Count;

                // 1体目は必ず順当に強い敵を出現させる(テーブルの一番最後には弱い敵を配置してあるから、cntに-1する)
                if(i == 0)
                {
                    cnt--;
                }

                int index = Random.Range(0, cnt);

                int createEnemyNumber = _createEnemyTable.createEnemyTableData[index].EnemyNumber;

                GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.encountEnemyNumbers.Add(createEnemyNumber);
            }

            IsEncount = true;
            Destroy(gameObject, 0.9f); //一定時間で消す
        }
    }
}
