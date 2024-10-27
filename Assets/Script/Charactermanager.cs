using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3.0f;     // �������x
    public float runSpeed = 6.0f;      // ���鑬�x
    public float jumpForce = 5.0f;     // �W�����v�̗�
    public Transform cameraTransform;  // �J������Transform���Q��

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        // ���͂��擾
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // �J�����̌����Ɋ�Â��Ĉړ��������v�Z
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // ���������𖳎����āA�L�����N�^�[���n�ʂɕ��s�ɓ����悤��
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // ���͕����ɉ������ړ��x�N�g�����v�Z
        Vector3 move = forward * vertical + right * horizontal;

        // �ړ��ʂ�����ꍇ�A�L�����N�^�[���ړ������Ɍ�����
        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
        }

        // Shift�L�[�ő��鑬�x�ɕύX
        float speed = Input.GetButton("Dash") ? runSpeed : walkSpeed;

        // �A�j���[�V�����̐ݒ�
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
            animator.SetBool("Idle", false); // Idle��Ԃ��I�t�ɂ���
        }
        else
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Run", false);
            animator.SetBool("Walk", false);
        }

        // Rigidbody���g�����ړ�
        Vector3 moveVelocity = move.normalized * speed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    void Jump()
    {
        // �W�����v�g���K�[�̔���
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("���n");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");  // �W�����v�A�j���[�V�������g���K�[�ōĐ�
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // �L�����N�^�[���n�ʂɒ��n�������𔻒�
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
