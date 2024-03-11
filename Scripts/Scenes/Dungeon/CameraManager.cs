using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// カメラマネージャークラス。ダンジョンでマッププレイヤーに追従する
public class CameraManager : MonoBehaviour
{
    // 追従させる対象のマッププレイヤー
    [SerializeField] MapPlayerController player;

    // マップ端では追従しないよう、追従限界のオブジェクト
    [SerializeField] GameObject clampObj;

    // カメラ追従スピード
    [SerializeField] float cameraSpeed;

    // カメラの開始位置
    private Vector3 _startPosition;

    // 追従限界のMAX位置
    private Vector3 _max;

    // 追従限界のMIN位置
    private Vector3 _min;

    private void Start()
    {
        _startPosition = transform.position;
        _max = clampObj.GetComponent<Renderer>().bounds.max;
        _min = clampObj.GetComponent<Renderer>().bounds.min;

        // カメラの初期位置をプレイヤーに合わせる
        Vector3 vector3 = player.transform.position;
        vector3.z = _startPosition.z;
        vector3.x = Mathf.Clamp(vector3.x, _min.x, _max.x);
        vector3.y = Mathf.Clamp(vector3.y, _min.y, _max.y);

        transform.position = vector3;
    }

    private void Update()
    {
        // シーン移動中でなければカメラ位置を更新する(カメラぶれ防止)
        if (player.IsSceneChange == false)
        {
            Vector3 vector3 = player.transform.position;
            vector3.z = _startPosition.z;
            vector3.x = Mathf.Clamp(vector3.x, _min.x, _max.x);
            vector3.y = Mathf.Clamp(vector3.y, _min.y, _max.y);
            transform.position = Vector3.MoveTowards(transform.position, vector3, Time.deltaTime * cameraSpeed);
        }
    }

    // 追従させるマッププレイヤーを設定する
    public void SetMapPlayer(MapPlayerController mapPlayer)
    {
        player = mapPlayer;
    }
}
