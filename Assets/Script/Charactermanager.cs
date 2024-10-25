using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3.0f;  // 歩く速度
    public float runSpeed = 6.0f;   // 走る速度
    public float gravity = -9.81f;  // 重力
    public Transform cameraTransform; // カメラのTransformを参照

    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        // 入力を取得
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // カメラの向きに基づいて移動方向を計算
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // 水平方向を無視して、キャラクターが地面に平行に動くように
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // 入力方向に応じた移動ベクトルを計算
        Vector3 move = forward * vertical + right * horizontal;

        // 移動量がある場合、キャラクターを移動方向に向ける
        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
        }

        // Shiftキーで走る速度に変更
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // アニメーションの設定
        if (move.magnitude > 0.1f)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("Run", true);
                animator.SetBool("Walk", false);
            }
            else
            {
                animator.SetBool("Walk", true);
                animator.SetBool("Run", false);
            }
            animator.SetBool("Idle", false); // Idle状態をオフにする
        }
        else
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Run", false);
            animator.SetBool("Walk", false);
        }

        // Rigidbodyを使った移動
        Vector3 moveVelocity = move.normalized * speed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }
}
