using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
// マップの移動可能オブジェクトクラス
public class MapMoveObject : MonoBehaviour
{
    // 重なり不可のオブジェクトのレイヤー
    [SerializeField] LayerMask solidObjectLayer;

    // 移動予定先に他のオブジェクトが入らないよう場所予約するダミーオブジェクト
    [SerializeField] GameObject dummyPrefab;

    // 強制静止フラグ
    [SerializeField] bool IsDontMoveForce;

    // 移動判定する間隔(フレーム指定)
    [SerializeField] int moveJudgeDurationFrame;

    // 移動する確率(0～100%指定)
    [SerializeField] int moveOdds;

    // 1マス移動の速度(1フレーム当たりの距離)
    [SerializeField] float moveSpeed;

    // 次移動までの間隔(秒指定)
    [SerializeField] float moveNextDurationTime;

    // デフォルト向き(戦闘以外からのシーン遷移時)
    [SerializeField] Vector2 defaultDirection;

    // 移動禁止フラグ(メニュー表示中やイベント中等)
    public bool IsDontMove { set; get; } = false;

    // 戦闘エンカウントフラグ
    public bool IsEncount { set; get; } = false;

    // 移動中フラグ
    public bool IsMoving { set; get; } = false;

    // 移動タイミングを計るための、フレームカウンタ
    private int _moveFrameCount = 0;

    // 上・下・左・右の移動方向を格納するためのデータ
    private readonly Vector2[] _moveDirection = new Vector2[4];


    // 上・下・左・右の移動方向を格納する
    private void Awake()
    {
        _moveDirection[0] = Vector2.up;
        _moveDirection[1] = Vector2.down;
        _moveDirection[2] = Vector2.left;
        _moveDirection[3] = Vector2.right;
    }
   

    // 移動タイミングを計る。(戻り値trueで移動する)
    public bool MoveTimingJudge()
    {
        bool ret = false;

        if ((IsMoving == false) && (IsDontMove == false) && (IsDontMoveForce == false))
        {
            _moveFrameCount++;

            // フレーム毎に移動判定
            if ((_moveFrameCount % moveJudgeDurationFrame) == 0)
            {
                // 確率で移動する
                if (Random.Range(0, 100) < moveOdds)
                {
                    ret = true;
                }

                _moveFrameCount = 0;
            }

        }

        return ret;
    }


    // 上・下・左・右のどこかにランダムに移動する
    public void RundomDirectionMove()
    {
        int direction = Random.Range(0, 4);

        GetComponent<Animator>().SetFloat("InputX", _moveDirection[direction].x);
        GetComponent<Animator>().SetFloat("InputY", _moveDirection[direction].y);

        StartCoroutine(Move(new Vector2(_moveDirection[direction].x * Define.MAP_TILE_SIZE, _moveDirection[direction].y * Define.MAP_TILE_SIZE)));
    }


    // 移動する
    public IEnumerator DoMove(Vector3 targetPos)
    {
        // 他のキャラが移動してこないように移動先にダミーを作成する
        var dummy = Instantiate(dummyPrefab, targetPos, Quaternion.identity);

        while (((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) && IsEncount == false)// 限りなく0に近い値より大きくなった場合
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // 移動が完了したらダミーを削除する
        Destroy(dummy);
    }


    // 目的地に移動できるかを判定する
    public bool IsWalkable(Vector3 targetPos)
    {
        return Physics2D.OverlapCircle(targetPos, Define.MAP_OBJECT_COLLISION_DISTANCE, solidObjectLayer) == false;
    }


    // 向いている方向を取得する
    public void GetDirection(ref Vector2 direction)
    {
        direction.x = GetComponent<Animator>().GetFloat("InputX");
        direction.y = GetComponent<Animator>().GetFloat("InputY");
    }


    // 向く方向を設定する
    public void SetDirection(Vector2 direction)
    {
        GetComponent<Animator>().SetFloat("InputX", direction.x);
        GetComponent<Animator>().SetFloat("InputY", direction.y);
    }

    // デフォルトの向く方向を設定する(戦闘以外からのシーン遷移時)
    public void SetDefaultDirection()
    {
        GetComponent<Animator>().SetFloat("InputX", defaultDirection.x);
        GetComponent<Animator>().SetFloat("InputY", defaultDirection.y);
    }


    // 目標地に向く
    public void LookToward(Vector3 targetPos)
    {
        float xDiff = targetPos.x - transform.position.x;
        float yDiff = targetPos.y - transform.position.y;

        GetComponent<Animator>().SetFloat("InputX", Mathf.Clamp(xDiff, -1f, 1f));
        GetComponent<Animator>().SetFloat("InputY", Mathf.Clamp(yDiff, -1f, 1f));
    }


    // 移動を実行する
    private IEnumerator Move(Vector3 direction)
    {
        IsMoving = true;

        Vector3 targetPos = transform.position + direction;

        // もし移動できないなら終了する
        if (IsWalkable(targetPos) == false)
        {
            IsMoving = false;
            yield break;
        }

        yield return DoMove(targetPos);

        if (IsEncount == false)
        {
            transform.position = targetPos;
        }

        yield return new WaitForSeconds(moveNextDurationTime);

        IsMoving = false;
    }

}
