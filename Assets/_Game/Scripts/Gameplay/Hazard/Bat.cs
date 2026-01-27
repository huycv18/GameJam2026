using UnityEngine;

/// <summary>
/// Bat - Con dơi với behavior đặc biệt:
/// 1. Đuổi ngang theo player (chỉ di chuyển theo trục X)
/// 2. Khi gần player → lao thẳng với tốc độ cao
/// 3. Player nhảy lên là né được vì dơi bay ngang
/// </summary>
public class Bat : ChasingHazard
{
    [Header("Bat Behavior")]
    [Tooltip("Khoảng cách để trigger charge (lao thẳng)")]
    [SerializeField] private float chargeDistance = 3f;
    
    [Tooltip("Tốc độ khi charge (lao thẳng)")]
    [SerializeField] private float chargeSpeed = 12f;
    
    [Tooltip("Độ cao bay (Y position cố định)")]
    [SerializeField] private float flyHeight = 2f;
    
    [Tooltip("Khoảng cách tối đa bay xa trước khi quay lại (giảm xuống nếu bat bay xa quá)")]
    [SerializeField] private float maxChargeDistance = 10f;
    
    [Tooltip("Thời gian tối đa charge trước khi quay lại (giây)")]
    [SerializeField] private float maxChargeTime = 2f;

    private BatState currentState = BatState.Chasing;
    private Vector3 chargeDirection;
    private Vector3 chargeStartPosition;
    private float chargeStartTime;
    private float initialY;

    private void Start()
    {
        // Lưu độ cao ban đầu của dơi
        initialY = transform.position.y;
    }

    protected override void Update()
    {
        // Tìm player nếu chưa có
        if (playerTransform == null)
        {
            FindPlayer();
            return;
        }

        // Xử lý state theo behavior của dơi
        switch (currentState)
        {
            case BatState.Chasing:
                HandleChasing();
                break;
            case BatState.Charging:
                HandleCharging();
                break;
        }
    }

    /// <summary>
    /// Đuổi theo player chỉ theo trục X (bay ngang)
    /// </summary>
    private void HandleChasing()
    {
        if (playerTransform == null) return;

        // Vị trí target: chỉ đuổi theo X, giữ nguyên Y
        Vector3 targetPos = new Vector3(
            playerTransform.position.x, 
            initialY + flyHeight,  // Giữ độ cao cố định
            transform.position.z
        );

        // Di chuyển ngang về phía player
        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetPos, 
            moveSpeed * Time.deltaTime
        );

        // Flip sprite theo hướng di chuyển
        if (playerTransform.position.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1); // Quay trái
        else
            transform.localScale = new Vector3(1, 1, 1);  // Quay phải

        // Kiểm tra khoảng cách để chuyển sang Charge
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer < chargeDistance)
        {
            // Tính hướng lao thẳng (chỉ theo trục X)
            chargeDirection = new Vector3(
                Mathf.Sign(playerTransform.position.x - transform.position.x), 
                0, 
                0
            );
            chargeStartPosition = transform.position;
            chargeStartTime = Time.time;
            currentState = BatState.Charging;
        }
    }

    /// <summary>
    /// Lao thẳng theo hướng đã định (không đổi hướng)
    /// </summary>
    private void HandleCharging()
    {
        // Bay thẳng ngang với tốc độ cao
        //Tính khoảng cách đã bay từ vị trí bắt đầu charge
        float distanceTraveled = Vector3.Distance(transform.position, chargeStartPosition);
        float timeElapsed = Time.time - chargeStartTime;

        // Reset về Chasing nếu:
        // 1. Bay xa quá maxChargeDistance HOẶC
        // 2. Charge quá lâu (maxChargeTime)
        if (distanceTraveled > maxChargeDistance || timeElapsed > maxChargeTime)
        // Reset state nếu bay ra ngoài màn hình
        if (Mathf.Abs(transform.position.x) > 20f)
        {
            currentState = BatState.Chasing;
        }
    }

    /// <summary>
    /// Visualize charge distance và flight path
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Vẽ phạm vi charge
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chargeDistance);

        // Vẽ line độ cao bay
        Gizmos.color = Color.cyan;
        if (Application.isPlaying)
        {
            Vector3 leftPoint = new Vector3(transform.position.x - 5, initialY + flyHeight, 0);
            Vector3 rightPoint = new Vector3(transform.position.x + 5, initialY + flyHeight, 0);
            Gizmos.DrawLine(leftPoint, rightPoint);
        }
    }

    private enum BatState
    {
        Chasing,
        Charging
    }
}
