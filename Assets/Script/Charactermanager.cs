using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3.0f;  // �������x
    public float runSpeed = 6.0f;   // ���鑬�x
    public float gravity = -9.81f;  // �d��
    public Transform cameraTransform; // �J������Transform���Q��

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
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // �A�j���[�V�����̐ݒ�
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
}
