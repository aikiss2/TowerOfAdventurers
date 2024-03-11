using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ダンジョンシーンのメインループクラス
public class DungeonManager : SceneManagerBase
{
    [System.Serializable]
    // マッププレイヤーのスタート位置
    private struct MapPlayerStartPos
    {
        // 前シーン
        public SceneName beforeScene;

        // オブジェクト番号
        public SceneChangeObjectNumber objectNumber;

        // スタート位置
        public Vector3 pos;

        // スタート向き
        public Vector2 direction;
    }

    // ダンジョンシーンのオブジェクト
    [SerializeField] DungeonObjects _dungeonObjects;

    // デフォルトBGM
    [field: SerializeField] public BGMType DefaultBgm { get; private set; }

    // マッププレイヤーのプレハブ
    [SerializeField] MapPlayerController mapPlayerPrefab;

    // マップエネミーのプレハブリスト
    [SerializeField] List<MapEnemyController> mapEnemyPrefabs = new();

    // バトルで出現する敵テーブルリスト
    [SerializeField] List<CreateEnemyTableSO> createEnemyTable = new();

    // シーン遷移テーブル
    public SceneChangeTableSO sceneChangeTableSO;

    // マッププレイヤーのスタート位置
    [SerializeField] List<MapPlayerStartPos> mapPlayerStartPos = new();

    // ダンジョンシーンの状態
    DungeonPhaseBase _phaseState;

    // 現在のシーン
    [field: SerializeField] public SceneName NowScene { get; private set; }
 
    // トーク番号(会話コマンドで話す内容)
    [field: SerializeField] public TalkTableIndex DungeonTalk { get; private set; }

    // 通常バトルで再生するBGM
    [field: SerializeField] public BGMType NormalBattleBgm { get; private set; }

    // 通常バトルの背景
    [field: SerializeField] public Sprite NormalBattleBackground { get; private set; }

    // ダンジョンマネージャーを保存するstatic変数
    public static DungeonManager dungeonManager;


    // static変数に自身を保存し、マップ上のキャラクターを生成する
    private void Awake()
    {
        dungeonManager = this;

        // バトルから戻ったら、バトル前の状態でプレイヤーと敵を生成、キャラクターを設定する
        if (Utility.beforeSceneName == SceneName.Battle)
        {
            // マッププレイヤー生成
            _dungeonObjects.mapPlayer = Instantiate(mapPlayerPrefab, Utility.mapPlayerMemory.pos, Quaternion.identity);
            _dungeonObjects.mapPlayer.GetComponent<MapMoveObject>().SetDirection(Utility.mapPlayerMemory.direction);
            _dungeonObjects.mapPlayer.NoMoveRect = _dungeonObjects.openMenuButton.GetComponent<RectTransform>();

            // マップエネミー生成
            foreach (var one in Utility.mapEnemiesMemory)
            {
                MapEnemyController mapEnemy = Instantiate(mapEnemyPrefabs[one.power], one.pos, Quaternion.identity);
                mapEnemy.SetEnemyTable(createEnemyTable[one.power], one.power);
                _dungeonObjects.mapEnemies.Add(mapEnemy);
            }

            int count = 0;
            // マップキャラクターは位置と向きを設定
            foreach (var one in Utility.mapCharactersMemory)
            {
                _dungeonObjects.mapCharacters[count].transform.position = one.pos;
                _dungeonObjects.mapCharacters[count].SetDirection(one.direction);
                count++;
            }

            // マップエネミーとマップキャラクターの一時保存データをクリア
            Utility.MapEnemyAndCharacterClear();
        }
        // バトル以外からの遷移は、マッププレイヤーのみ生成する。キャラクターは向きのみ設定
        else
        {
            // 念のため初期位置を設定しておく
            var startPos = mapPlayerStartPos[0].pos;
            var startDirection = mapPlayerStartPos[0].direction;

            // 前シーンとオブジェクト番号で、プレイヤーの初期位置を決める
            foreach (var one in mapPlayerStartPos)
            {
                if( (one.beforeScene == Utility.beforeSceneName) && (one.objectNumber == Utility.nextSceneObjectNumber))
                {
                    startPos = one.pos;
                    startDirection = one.direction;
                    break;
                }
            }

            // プレイヤーを生成する
            _dungeonObjects.mapPlayer = Instantiate(mapPlayerPrefab, startPos, Quaternion.identity);
            _dungeonObjects.mapPlayer.GetComponent<MapMoveObject>().SetDirection(startDirection);
            _dungeonObjects.mapPlayer.NoMoveRect = _dungeonObjects.openMenuButton.GetComponent<RectTransform>();

            // プレイヤー以外のキャラクターの向きを設定する
            foreach (var one in _dungeonObjects.mapCharacters)
            {
                one.SetDefaultDirection();
            }
        }

        // カメラにマッププレイヤー登録
        Camera.main.GetComponent<CameraManager>().SetMapPlayer(_dungeonObjects.mapPlayer);
    }


    // シーンの初期設定をし、メインループを開始する
    private void Start()
    {
        GlobalCanvasManager.globalCanvasManager.SetCamera(mainCamera);
        Utility.nextScene = SceneName.None;
        Utility.nextSceneObjectNumber = SceneChangeObjectNumber.Number0;
        Fadein();

        _phaseState = new DungeonStartPhase();
        StartCoroutine(Main());
    }


    // メインループ。ENDフェーズに遷移したら終了する
    private IEnumerator Main()
    {
        while (_phaseState is not DungeonEndPhase)
        {
            yield return _phaseState.Execute(_dungeonObjects);

            _phaseState = _phaseState.next;
        }

        yield return _phaseState.Execute(_dungeonObjects);
    }


    // マップエネミーを生成する
    public void CreateEnemy()
    {
        // 既にマップエネミーが最大数に達している場合は、何もせず終了する
        if (_dungeonObjects.mapEnemies.Count >= Define.DUNGEON_ENEMY_MAX)
        {
            return;
        }

        // 生成する敵の強さを決める
        int power = Random.Range(0, createEnemyTable.Count);

        // オブジェクトが干渉しない地点かどうかを判定しながら生成場所を決める。
        // 一定回数場所を探し、なかった場合は生成をあきらめる
        for (int i = 0; i < Define.DUNGEON_CREATE_ENEMY_TRY_MAX; i++)
        {
            // プレイヤーの上下左右ランダムに生成するための値
            float minus_x = -1.0f;
            float minus_y = -1.0f;
            if (Random.Range(0, 2) == 0) { minus_x = 1.0f; }
            if (Random.Range(0, 2) == 0) { minus_y = 1.0f; }

            // プレイヤーから数マス分近くから敵を生成
            var pos = new Vector3(_dungeonObjects.mapPlayer.transform.position.x - (_dungeonObjects.mapPlayer.transform.position.x % Define.MAP_TILE_SIZE) + 
                                (Define.MAP_TILE_SIZE * Random.Range(Define.DUNGEON_CREATE_ENEMY_DISTANCE_X_MIN, Define.DUNGEON_CREATE_ENEMY_DISTANCE_X_MAX) * minus_x), 
                                _dungeonObjects.mapPlayer.transform.position.y - (_dungeonObjects.mapPlayer.transform.position.y % Define.MAP_TILE_SIZE) + 
                                (Define.MAP_TILE_SIZE * Random.Range(Define.DUNGEON_CREATE_ENEMY_DISTANCE_Y_MIN, Define.DUNGEON_CREATE_ENEMY_DISTANCE_Y_MAX) * minus_y), 0);

            // 生成できる場所かを判定し、生成可能であればマップエネミーを生成する
            if (mapEnemyPrefabs[power].GetComponent<MapMoveObject>().IsWalkable(pos))
            {
                MapEnemyController mapEnemy = Instantiate(mapEnemyPrefabs[power], pos, Quaternion.identity);
                mapEnemy.SetEnemyTable(createEnemyTable[power], power);
                _dungeonObjects.mapEnemies.Add(mapEnemy);
                break;
            }
        }
    }


    // マップ上の全てのキャラクター(プレイヤー・敵・キャラ)の情報を保存する
    public void MapAllCharacterMemory()
    {
        // マッププレイヤー保存
        Utility.mapPlayerMemory.pos = _dungeonObjects.mapPlayer.transform.position;
        _dungeonObjects.mapPlayer.GetComponent<MapMoveObject>().GetDirection(ref Utility.mapPlayerMemory.direction);

        // マップエネミー保存
        foreach (MapEnemyController enemy in _dungeonObjects.mapEnemies)
        {
            if(enemy != null)
            {
                MapEnemyMemory mapEnemy;
                mapEnemy.power = enemy.Power;
                mapEnemy.pos = enemy.transform.position;

                Utility.mapEnemiesMemory.Add(mapEnemy);
            }
        }

        // マップキャラクター保存
        foreach (var one in _dungeonObjects.mapCharacters)
        {
            MapCharaMemory mapChara;
            mapChara.pos = one.transform.position;
            mapChara.direction = new Vector2(0,0);
            one.GetDirection(ref mapChara.direction);

            Utility.mapCharactersMemory.Add(mapChara);
        }
    }


    // ダンジョンシーンのオブジェクトを取得する
    public DungeonObjects GetDungeonObjects()
    {
        return _dungeonObjects;
    }
}


[System.Serializable]
// ダンジョンシーンのオブジェクト
public struct DungeonObjects
{
    // マッププレイヤーのオブジェクト
    public MapPlayerController mapPlayer;

    // マップエネミーのオブジェクト
    public List<MapEnemyController> mapEnemies;

    // マップキャラクターのオブジェクト
    public List<MapMoveObject> mapCharacters;

    // メニューボタン
    public SelectableButton openMenuButton;

    // 項目選択パネル
    public SelectButtonPanel selectButtonPanel;

    // マップスキル選択パネル
    public MapSkillPanel mapSkillPanel;

    // キャラ図鑑の名前パネル
    public CharaNamePanel charaNamePanel;

    // キャラ図鑑のスキルパネル
    public SkillPanel charaSkillPanel;

    // キャラ図鑑のキャラ絵パネル
    public CharaImagePanel charaImagePanel;

    // キャラ図鑑のステータスパネル
    public StatusPanel statusPanel;

    // キャラ図鑑のキャラ説明パネル
    public CharaStoryPanel charaStoryPanel;

    // バトル開始のフェードアウト
    public Fade battleFade;
}

