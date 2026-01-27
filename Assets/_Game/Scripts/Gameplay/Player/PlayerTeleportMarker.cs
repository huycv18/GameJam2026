using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerTeleportMarker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerConfig config;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject markerPrefab;
    
    private PlayerInput _input;
    private PlayerMovement _movement;
    private PlayerJump _jump;
    private PlayerDash _dash;
    private Rigidbody2D _rb;
    
    private TeleportMarker _activeMarker;
    private float _teleportWindowTimer;
    private float _cooldownTimer;
    private bool _canTeleport;
    
    public bool CanUseSkill => _cooldownTimer <= 0f && _activeMarker == null;
    public bool CanTeleport => _canTeleport && _activeMarker != null;
    public float CooldownPercent => Mathf.Clamp01(1f - (_cooldownTimer / config.TeleportCooldown));
    
    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _movement = GetComponent<PlayerMovement>();
        _jump = GetComponent<PlayerJump>();
        _dash = GetComponent<PlayerDash>();
        _rb = GetComponent<Rigidbody2D>();
        
        if (config == null)
        {
            Debug.LogError("PlayerConfig not assigned to PlayerTeleportMarker!", this);
        }
        
        if (markerPrefab == null)
        {
            Debug.LogError("Marker prefab not assigned to PlayerTeleportMarker!", this);
        }
    }
    
    void OnEnable()
    {
        _input.On<bool>(PlayerInputType.Skill, OnSkillInput);
    }
    
    void OnDisable()
    {
        _input.Off<bool>(PlayerInputType.Skill, OnSkillInput);
    }
    
    void Update()
    {
        UpdateCooldown();
        UpdateTeleportWindow();
    }
    
    private void OnSkillInput(bool pressed)
    {
        if (!pressed) return;
        
        if (_activeMarker == null && CanUseSkill)
        {
            ThrowMarker();
        }
        else if (CanTeleport)
        {
            TeleportToMarker();
        }
    }
    
    private void ThrowMarker()
    {
        if (config == null || markerPrefab == null) return;
        
        Vector2 direction = CalculateThrowDirection();
        
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        GameObject markerObj = Instantiate(markerPrefab, spawnPos, Quaternion.identity);
        
        _activeMarker = markerObj.GetComponent<TeleportMarker>();
        if (_activeMarker != null)
        {
            _activeMarker.Initialize(config, direction, transform);
            
            _activeMarker.OnLanded += OnMarkerLanded;
            _activeMarker.OnPickedUp += OnMarkerPickedUp;
            _activeMarker.OnExpired += OnMarkerExpired;
        }
        else
        {
            Debug.LogError("Marker prefab missing TeleportMarker component!");
            Destroy(markerObj);
        }
    }
    
    private Vector2 CalculateThrowDirection()
    {
        float facingAngle = _movement.FacingDirection > 0 ? 0f : 180f;
        float totalAngle = facingAngle + config.MarkerThrowAngle;
        
        float radians = totalAngle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
    }
    
    private void OnMarkerLanded(TeleportMarker marker)
    {
        _canTeleport = true;
        _teleportWindowTimer = config.TeleportWindowTime;
    }
    
    private void OnMarkerPickedUp(TeleportMarker marker)
    {
        _cooldownTimer = 0f;
        _activeMarker = null;
        _canTeleport = false;
    }
    
    private void OnMarkerExpired(TeleportMarker marker)
    {
        _activeMarker = null;
        _canTeleport = false;
    }
    
    private void TeleportToMarker()
    {
        if (_activeMarker == null) return;
        
        Vector2 targetPos = _activeMarker.Position;
        Vector2 validPos = FindValidTeleportPosition(targetPos);
        
        _rb.velocity = Vector2.zero;
        
        if (_dash != null && _dash.IsDashing)
        {
            _dash.CancelDash();
        }
        
        transform.position = validPos;
        
        Destroy(_activeMarker.gameObject);
        _activeMarker = null;
        _canTeleport = false;
        
        _cooldownTimer = config.TeleportCooldown;
    }
    
    private Vector2 FindValidTeleportPosition(Vector2 targetPos)
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            return targetPos;
        }
        
        if (!IsPositionBlocked(targetPos, playerCollider))
        {
            return targetPos;
        }
        
        for (int i = 1; i <= config.TeleportMaxAttempts; i++)
        {
            Vector2 offsetPos = targetPos + Vector2.up * (config.TeleportOffsetY * i);
            
            if (!IsPositionBlocked(offsetPos, playerCollider))
            {
                return offsetPos;
            }
        }
        
        Debug.LogWarning("Cannot find valid teleport position, staying at current position!");
        return transform.position;
    }
    
    private bool IsPositionBlocked(Vector2 pos, Collider2D playerCollider)
    {
        float checkRadius = Mathf.Max(
            playerCollider.bounds.extents.x, 
            playerCollider.bounds.extents.y
        );
        
        Collider2D[] overlaps = Physics2D.OverlapCircleAll(pos, checkRadius);
        
        foreach (var overlap in overlaps)
        {
            if (overlap != playerCollider && !overlap.isTrigger)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private void UpdateTeleportWindow()
    {
        if (!_canTeleport) return;
        
        _teleportWindowTimer -= Time.deltaTime;
        
        if (_teleportWindowTimer <= 0f)
        {
            _canTeleport = false;
        }
    }
    
    private void UpdateCooldown()
    {
        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
        }
    }
    
    void OnDrawGizmos()
    {
        if (firePoint != null && config != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, 0.1f);
            
            if (Application.isPlaying && _movement != null)
            {
                Vector2 dir = CalculateThrowDirection();
                Gizmos.color = CanUseSkill ? Color.green : Color.red;
                Gizmos.DrawRay(firePoint.position, dir * 2f);
            }
        }
    }
}
