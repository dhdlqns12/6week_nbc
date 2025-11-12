using UnityEngine;

public class BouncePad : MonoBehaviour, IObject
{
    [Header("점프대 설정")]
    [SerializeField] private float bounceForce;
    [SerializeField] private Vector3 bounceDirection;

    [Header("오브젝트 정보")]
    [SerializeField] private string objectName = "점프대";
    [SerializeField] private string description = "플레이어를 높이 튕겨냅니다";

    private Rigidbody playerRb;
    private PlayerFSM playerFSM;
    private Transform playerTransform;

    public string ObjectName => objectName;
    public string Description => description;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (playerRb == null)
        {
            playerRb = other.GetComponent<Rigidbody>();
            playerFSM = other.GetComponent<PlayerFSM>();
            playerTransform = other.transform;
        }

        if (playerRb != null)
        {
            playerRb.velocity = Vector3.zero;

            Vector3 finalDirection;

            finalDirection = playerTransform.forward + Vector3.up;
            finalDirection = finalDirection.normalized;

            playerRb.AddForce(finalDirection * bounceForce, ForceMode.Impulse);

            if (playerFSM != null)
            {
                playerFSM.ChangeState(PlayerFSM.PlayerState.Jump);
            }
        }
    }
}
