using UnityEngine;

public class PlayerController : MonoBehaviour
{
  public Player player;
  public static float horizontalInput;
  public CollisionManager collisionManager;
  private Rigidbody2D _rigidBody;
  private BoxCollider2D _boxCollider;
  public static IdleState idleState;
  public static WalkingState walkingState;
  public static JumpingState jumpingState;
  public static AirNearWallState airNearWallState;
  public static AirNearWallWithExtraInputState airNearWallWithExtraInputState;
  public static FallingFreeFromWallState fallingFreeFromWallState;
  public static GrabbingWallState grabbingWallState;
  public static RecoilFromWallWithHorizontalEffectState recoilFromWallWithHorizontalEffectState;

  public static RecoilFromWallState recoilFromWallState;
  private BaseState _currentState;
  private void Awake()
  {
    _rigidBody = GetComponent<Rigidbody2D>();
    _boxCollider = GetComponent<BoxCollider2D>();
    player = new Player(_boxCollider, transform);
    collisionManager = new CollisionManager(player);

    PlayerController.idleState = new IdleState("Idle", this, player);
    PlayerController.walkingState = new WalkingState("Walking", this, player);
    PlayerController.jumpingState = new JumpingState("Jumping", this, player);
    PlayerController.airNearWallState = new AirNearWallState("AirNearWallState", this, player);
    PlayerController.fallingFreeFromWallState = new FallingFreeFromWallState("FallingFreeFromState", this, player);
    PlayerController.grabbingWallState = new GrabbingWallState("GrabbingWallState", this, player);
    PlayerController.recoilFromWallState = new RecoilFromWallState("RecoilFromState",this, player);
    PlayerController.recoilFromWallWithHorizontalEffectState = new RecoilFromWallWithHorizontalEffectState("RecoilFromWallWithHorizontalEffectState",this,player);
    PlayerController.airNearWallWithExtraInputState = new AirNearWallWithExtraInputState("AirNearWallWithExtraInputState", this, player);
    //* 
    Initialize();
  }
  void Initialize()
  {
    _currentState = PlayerController.idleState;
    _currentState.Enter();
  }
  public void ChangeState(BaseState newState)
  {
    _currentState.Exit();
    _currentState = newState;
    newState.Enter();
  }
  void HandleInput()
  {
    PlayerController.horizontalInput = Input.GetAxis("Horizontal");
    _currentState.HandleInput();
  }
  void Update()
  {
    HandleInput();
    _currentState.LogicUpdate();
  }
  void FixedUpdate()
  {
    _currentState.PhysicsUpdate();
    _rigidBody.velocity = player.velocity;
  }
}
