using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;                // プレイヤーのTransform
    public float smoothSpeed = 0.125f;      // 追従のスムーズさ
    public float rotationSpeed = 5.0f;      // カメラ回転速度

    public float height = 3.0f;             // カメラの高さを調整するための変数
    public Vector3 focusOffset = new Vector3(0, 1.5f, 0); // フォーカス位置のオフセット

    private Vector3 offset;                 // 初期のオフセット
    private float currentX = 0.0f;          // マウスの水平入力
    private float currentY = 0.0f;          // マウスの垂直入力
    public float yMinLimit = -20f;          // 垂直方向の下限
    public float yMaxLimit = 80f;           // 垂直方向の上限

    void Start()
    {
        // プレイヤーとカメラの初期オフセットを取得
        offset = transform.position - player.position;

        // Y成分をカメラの高さで設定
        offset.y = height; // ここでheightを使ってオフセットを設定
    }

    void LateUpdate()
    {

        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
        //currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // 垂直方向の角度制限
        currentY = Mathf.Clamp(currentY, yMinLimit, yMaxLimit);


        // 回転に基づいて新しいオフセットを計算
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 rotatedOffset = rotation * offset;

        // プレイヤーの位置に基づいた目標位置を計算
        Vector3 targetPosition = player.position + rotatedOffset;

        // カメラの位置をスムーズに補間
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

       

        // フォーカス位置を設定
        Vector3 focusPosition = player.position + focusOffset;
        transform.LookAt(focusPosition);
    }

    void Update()
    {
        // heightをエディターで調整した場合にオフセットを再設定
        offset.y = height;
    }
}
