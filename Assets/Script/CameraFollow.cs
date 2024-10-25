using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;                // �v���C���[��Transform
    public float smoothSpeed = 0.125f;      // �Ǐ]�̃X���[�Y��
    public float rotationSpeed = 5.0f;      // �J������]���x

    public float height = 3.0f;             // �J�����̍����𒲐����邽�߂̕ϐ�
    public Vector3 focusOffset = new Vector3(0, 1.5f, 0); // �t�H�[�J�X�ʒu�̃I�t�Z�b�g

    private Vector3 offset;                 // �����̃I�t�Z�b�g
    private float currentX = 0.0f;          // �}�E�X�̐�������
    private float currentY = 0.0f;          // �}�E�X�̐�������
    public float yMinLimit = -20f;          // ���������̉���
    public float yMaxLimit = 80f;           // ���������̏��

    void Start()
    {
        // �v���C���[�ƃJ�����̏����I�t�Z�b�g���擾
        offset = transform.position - player.position;

        // Y�������J�����̍����Őݒ�
        offset.y = height; // ������height���g���ăI�t�Z�b�g��ݒ�
    }

    void LateUpdate()
    {

        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
        //currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // ���������̊p�x����
        currentY = Mathf.Clamp(currentY, yMinLimit, yMaxLimit);


        // ��]�Ɋ�Â��ĐV�����I�t�Z�b�g���v�Z
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 rotatedOffset = rotation * offset;

        // �v���C���[�̈ʒu�Ɋ�Â����ڕW�ʒu���v�Z
        Vector3 targetPosition = player.position + rotatedOffset;

        // �J�����̈ʒu���X���[�Y�ɕ��
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

       

        // �t�H�[�J�X�ʒu��ݒ�
        Vector3 focusPosition = player.position + focusOffset;
        transform.LookAt(focusPosition);
    }

    void Update()
    {
        // height���G�f�B�^�[�Œ��������ꍇ�ɃI�t�Z�b�g���Đݒ�
        offset.y = height;
    }
}
