using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;
    public float jumpForce = 5.0f;
    public Transform cameraTransform;
    public GameObject effectPrefab;  // �G�t�F�N�g�̃v���n�u���C���X�y�N�^�[�Őݒ�
    public Vector3 effectOffset = new Vector3(0, 1, 1);  // �G�t�F�N�g�̈ʒu�I�t�Z�b�g�i�L�����N�^�[�̐��ʁj
    public Quaternion effectRotation = Quaternion.identity;  // �G�t�F�N�g�̉�]
    public Vector3 effectScale = new Vector3(1, 1, 1);  // �G�t�F�N�g�̃X�P�[��

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;
    private GameObject effectInstance;  // ���������G�t�F�N�g��ێ�����ϐ�

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
            Debug.Log("�z������");
            animator.SetBool("Target pull", true);

            // �G�t�F�N�g�̐����i����̂݁j
            if (effectInstance == null && effectPrefab != null)
            {
                effectInstance = Instantiate(effectPrefab, transform.position + transform.forward + effectOffset, effectRotation);
                effectInstance.transform.localScale = effectScale;  // �X�P�[����ݒ�
            }

            // �G�t�F�N�g���L�����N�^�[�̈ʒu�ƌ����ɒǏ]������
            if (effectInstance != null)
            {
                effectInstance.transform.position = transform.position + transform.forward * effectOffset.z + new Vector3(0, effectOffset.y, 0);
                effectInstance.transform.rotation = transform.rotation * effectRotation;
            }

            // ��ԋ߂�enemy�������āA���̕���������
            GameObject closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                Vector3 directionToEnemy = closestEnemy.transform.position - transform.position;
                directionToEnemy.y = 0; // �L�����N�^�[�������ɉ�]����悤��y�����Œ�

                Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);
            }
        }
        else
        {
            animator.SetBool("Target pull", false);

            // �G�t�F�N�g���폜
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
            Debug.Log("���n");
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
