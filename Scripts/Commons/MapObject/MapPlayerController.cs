using DG.Tweening.Core;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

[RequireComponent(typeof(MapMoveObject))]
// マップのプレイヤークラス
public class MapPlayerController : MonoBehaviour
{
    // プレイヤーが交流できるオブジェクトのレイヤー
    [SerializeField] LayerMask interactLayer;

    // マップの敵生成フラグ(移動した際に生成判定)
    public bool IsCreateEnemy { get; set; }

    // シーン移動フラグ
    public bool IsSceneChange { get; private set; }

    // 次シーンの番号
    public int NextSceneNo {  get; set; }

    // クリック移動において、移動判定しない矩形(メニューボタンのクリックでは移動させない)
    public RectTransform NoMoveRect { get; set; }

    // 戦闘エンカウントフラグ
    public bool IsEncount
    {
        set { mapMoveObject.IsEncount = value; }
        get { return mapMoveObject.IsEncount; }
    }

    // 移動禁止フラグ(メニュー表示中やイベント中等)
    public bool IsDontMove
    {
        set { mapMoveObject.IsDontMove = value; }
        get { return mapMoveObject.IsDontMove; }
    }

    // 移動中フラグ
    public bool IsMoving
    {
        set { mapMoveObject.IsMoving = value; }
        get { return mapMoveObject.IsMoving; }
    }

    // マップの移動可能オブジェクトを制御するためのコンポーネント
    [SerializeField] private MapMoveObject mapMoveObject;


    // プレイヤーが交流できるオブジェクトが目の前にあるか判定する(true:交流できる)
    public bool PlayerActionAble()
    {
        bool ret = false;

        Vector3 faceDirection = new Vector3(GetComponent<Animator>().GetFloat("InputX"), GetComponent<Animator>().GetFloat("InputY"));

        Vector3 interactPos = transform.position + faceDirection;

        Collider2D collider2D = Physics2D.OverlapCircle(interactPos, Define.MAP_OBJECT_COLLISION_DISTANCE, interactLayer);
        if (collider2D)
        {
            ret = collider2D.GetComponent<IInteract>().GetInteractAble();
        }

        return ret;
    }


    // プレイヤーアクションを実行する
    public IEnumerator PlayerAction()
    {
        Vector3 faceDirection = new(GetComponent<Animator>().GetFloat("InputX"), GetComponent<Animator>().GetFloat("InputY"));

        Vector3 interactPos = transform.position + faceDirection;

        Collider2D collider2D = Physics2D.OverlapCircle(interactPos, Define.MAP_OBJECT_COLLISION_DISTANCE, interactLayer);
        if (collider2D)
        {
            yield return collider2D.GetComponent<IInteract>().Interact(this);
        }

        yield break;
    }

 
    // 毎フレーム移動判定し、移動入力があれば移動する
    void Update()
    {
        if( (IsMoving == false) && (IsDontMove == false))
        {
            Vector2 moveDirection = Vector2.zero;

            if ( MoveJudge(ref moveDirection) == true )
            {
                GetComponent<Animator>().SetFloat("InputX", moveDirection.x);
                GetComponent<Animator>().SetFloat("InputY", moveDirection.y);

                StartCoroutine(Move(new Vector2(moveDirection.x * Define.MAP_TILE_SIZE, moveDirection.y * Define.MAP_TILE_SIZE)));
            }
        }

        GetComponent<Animator>().SetBool("isMoving", IsMoving);
    }


    // 移動判定をし、移動方向をmoveDirectionOutputに格納する。(戻り値true:移動する)
    private bool MoveJudge(ref Vector2 moveDirectionOutput)
    {
        // キーボードでの移動判定
        moveDirectionOutput.x = Input.GetAxisRaw("Horizontal");
        moveDirectionOutput.y = Input.GetAxisRaw("Vertical");

        // 横移動の入力を優先する
        if (moveDirectionOutput.x != 0)
        {
            moveDirectionOutput.y = 0;
        }

        // マウスクリックでの移動判定
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 0.0f;
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            float x_diff = clickPosition.x - transform.position.x;
            float y_diff = clickPosition.y - transform.position.y;

            // プレイヤーより少し離れた位置のクリックであれば移動する
            if ((Math.Abs(x_diff) >= 0.5f) || (Math.Abs(y_diff) >= 0.5f))
            {
                // より遠い方を優先的に移動する
                if (Math.Abs(x_diff) > Math.Abs(y_diff))
                {
                    moveDirectionOutput.x = Mathf.Clamp(x_diff * 2.0f, -1.0f, 1.0f); // -1か1になるように2倍している。
                    moveDirectionOutput.y = 0;
                }
                else
                {
                    moveDirectionOutput.x = 0;
                    moveDirectionOutput.y = Mathf.Clamp(y_diff * 2.0f, -1.0f, 1.0f);
                }
            }

            var corners = new Vector3[4];
            NoMoveRect.GetWorldCorners(corners);

            // メニューボタンを押した時は動かないようにする
            Rect rect = new(corners[0].x, corners[0].y, corners[3].x - corners[0].x, corners[1].y - corners[0].y);
            if (rect.Contains(clickPosition))
            {
                moveDirectionOutput.x = 0;
                moveDirectionOutput.y = 0;
            }
        }

        if( (moveDirectionOutput.x == 0) && (moveDirectionOutput.y == 0))
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    // 移動を実行するコルーチン
    private IEnumerator Move(Vector3 direction)
    {
        IsMoving = true;

        Vector3 targetPos = transform.position + direction;

        // もし移動できないなら終了する
        if (mapMoveObject.IsWalkable(targetPos) == false)
        {
            IsMoving = false;
            yield break;
        }

        // 移動処理を委譲
        yield return mapMoveObject.DoMove(targetPos);

        // 敵と接触した場合
        if (IsEncount == true)
        {
            IsMoving = false;
            IsDontMove = true;
            yield return new WaitForSeconds(0.92f);//フェードアウト待ち
            transform.position = targetPos;
        }
        // シーン移動オブジェクトに接触した場合
        else if(IsSceneChange == true)
        {
            IsMoving = false;
            IsDontMove = true;
            yield return new WaitForSeconds(3f);//フェードアウト待ち(シーン終了まで待つ)
        }
        // メニュー等が開かれた場合
        else if (IsDontMove == true)
        {
            transform.position = targetPos;
            IsMoving = false;
        }
        else
        {
            // 確率で敵生成
            if (UnityEngine.Random.Range(0, 100) < Define.DUNGEON_CREATE_ENEMY_RATE)
            {
                IsCreateEnemy = true;
            }

            Vector2 moveDirection = Vector2.zero;

            // 継続して移動するかを判定
            if ( MoveJudge(ref moveDirection) == true )
            {
                // 継続移動の場合は、移動誤差を次の移動に加算する
                float add_x = targetPos.x - transform.position.x;
                float add_y = targetPos.y - transform.position.y;

                GetComponent<Animator>().SetFloat("InputX", moveDirection.x);
                GetComponent<Animator>().SetFloat("InputY", moveDirection.y);

                StartCoroutine(Move(new Vector2((moveDirection.x * Define.MAP_TILE_SIZE) + add_x, (moveDirection.y * Define.MAP_TILE_SIZE) + add_y)));
            }
            // 移動入力なければ目的地を設定する
            else
            {
                transform.position = targetPos;
                IsMoving = false;
            }
        }

        yield return null;
    }


    // 他のオブジェクトとの衝突判定(反発ありオブジェクト)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 敵オブジェクトとの諸突判定
        if (collision.gameObject.CompareTag("Enemy"))
        {
            NextSceneNo = Define.DUNGEON_NEXT_BATTLE_SCENE_NO;
            IsEncount = true;
        }
    }


    // 他のオブジェクトとの接触判定(反発なしオブジェクト)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // シーン移動オブジェクトとの接触判定(0～4すべて)
        if (collision.gameObject.CompareTag("SceneChangeObject0"))
        {
            NextSceneNo = 0;
            IsSceneChange = true;
        }
        else if (collision.gameObject.CompareTag("SceneChangeObject1"))
        {
            NextSceneNo = 1;
            IsSceneChange = true;
        }
        else if (collision.gameObject.CompareTag("SceneChangeObject2"))
        {
            NextSceneNo = 2;
            IsSceneChange = true;
        }
        else if (collision.gameObject.CompareTag("SceneChangeObject3"))
        {
            NextSceneNo = 3;
            IsSceneChange = true;
        }
        else if (collision.gameObject.CompareTag("SceneChangeObject4"))
        {
            NextSceneNo = 4;
            IsSceneChange = true;
        }

    }

}
