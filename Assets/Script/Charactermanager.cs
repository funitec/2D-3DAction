using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;
    public float jumpForce = 5.0f;
    public Transform cameraTransform;
    public GameObject effectPrefab;  // エフェクトのプレハブをインスペクターで設定
    public Vector3 effectOffset = new Vector3(0, 1, 1);  // エフェクトの位置オフセット（キャラクターの正面）
    public Quaternion effectRotation = Quaternion.identity;  // エフェクトの回転
    public Vector3 effectScale = new Vector3(1, 1, 1);  // エフェクトのスケール

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;
    private GameObject effectInstance;  // 生成したエフェクトを保持する変数

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Jump();
        Absorption();
    }

    void Absorption()
    {
        if (Input.GetButton("Target pull"))
        {
            Debug.Log("吸い込み");
            animator.SetBool("Target pull", true);

            // エフェクトの生成（初回のみ）
            if (effectInstance == null && effectPrefab != null)
            {
                effectInstance = Instantiate(effectPrefab, transform.position + transform.forward + effectOffset, effectRotation);
                effectInstance.transform.localScale = effectScale;  // スケールを設定
            }

            // エフェクトをキャラクターの位置と向きに追従させる
            if (effectInstance != null)
            {
                effectInstance.transform.position = transform.position + transform.forward * effectOffset.z + new Vector3(0, effectOffset.y, 0);
                effectInstance.transform.rotation = transform.rotation * effectRotation;
            }

            // 一番近いenemyを見つけて、その方向を向く
            GameObject closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                Vector3 directionToEnemy = closestEnemy.transform.position - transform.position;
                directionToEnemy.y = 0; // キャラクターが水平に回転するようにy軸を固定

                Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);
            }
        }
        else
        {
            animator.SetBool("Target pull", false);

            // エフェクトを削除
            if (effectInstance != null)
            {
                Destroy(effectInstance);
                effectInstance = null;
            }
        }
    }

    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * vertical + right * horizontal;

        if (move.magnitude > 0.1f && Input.GetButton("Target pull") == false)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
        }

        float speed = Input.GetButton("Dash") ? runSpeed : walkSpeed;

        if (move.magnitude > 0.1f)
        {
            if (Input.GetButton("Dash"))
            {
                animator.SetBool("Run", true);
                animator.SetBool("Walk", false);
            }
            else
            {
                animator.SetBool("Walk", true);
                animator.SetBool("Run", false);
            }
            animator.SetBool("Idle", false);
        }
        else
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Run", false);
            animator.SetBool("Walk", false);
        }

        Vector3 moveVelocity = move.normalized * speed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    void Jump()
    {
        if (Input.GetButton("Jump") && isGrounded)
        {
            Debug.Log("着地");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
