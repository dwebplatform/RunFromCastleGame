using UnityEngine;
public class AirNearWallState : GroundedState
{
  private PlayerController _playerController;
  private float _jumpForce = 5f;
  private Player _player;
  private float _startTime;

  private HittedParams hittedParams;
  public AirNearWallState(string name, PlayerController playerController, Player player) : base(name, playerController, player)
  {
    _playerController = playerController;
    _player = player;
  }
  public override void Enter()
  {
    base.Enter();
    _player.velocity = new Vector2(0f, _jumpForce);
    _startTime = Time.time;
    _isGrounded = false;
    _player.closestWall = null;
  }
  private bool IsObstacleIgnored()
  {
    float deltaTime = Time.time - _startTime;
    if (deltaTime < 0.2)
    {
      return true;
    }
    return false;
  }
  public override void HandleInput()
  {
    base.HandleInput();
  }

  public override void LogicUpdate()
  {
    base.LogicUpdate();

    float horizontalInput = PlayerController.horizontalInput;
    bool isObstacleIgnored = IsObstacleIgnored();
    if (_player.closestWall != null)
    {
      Vector2 normal = _player.closestWall.normal;
      if (Mathf.Abs(PlayerController.horizontalInput) > Mathf.Epsilon)
      {
        if (CollisionUtils.IsWallInFront(normal, PlayerController.horizontalInput))
        {
          
        }
        else
        {
          _playerController.ChangeState(PlayerController.fallingFreeFromWallState);
        }
      }
    }
    if (_isGrounded && !isObstacleIgnored)
    {
      _playerController.ChangeState(PlayerController.idleState);
    }
  }
  public override void PhysicsUpdate()
  {
    bool isObstacleIgnored = IsObstacleIgnored();
    if (!isObstacleIgnored)
    {
      base.PhysicsUpdate();
    }

    if (!_isGrounded)
    {
      _player.velocity.y -= Player.GRAVITY * Time.fixedDeltaTime;
    }
    if (!isObstacleIgnored)
    {
      //   _player.closestWall
      _playerController.collisionManager.WallCheck(Player.WALL_OFFSET,
 (rightCollider, isHitted) =>
{
  hittedParams.isHittedRight = isHitted;
  if (!isHitted)
  {
    return;
  }

  _player.closestWall = new WallInfo();
  _player.closestWall.collider = rightCollider.collider;
  _player.closestWall.normal = rightCollider.normal;
  _player.transform.position = CollisionUtils.AdjustPositionRight(rightCollider, _player);
},
(leftCollidedInfo, isHitted) =>
{
  hittedParams.isHittedLeft = isHitted;
  if (!isHitted)
  {
    return;
  }
  _player.closestWall = new WallInfo();
  _player.closestWall.collider = leftCollidedInfo.collider;
  _player.closestWall.normal = leftCollidedInfo.normal;
  _player.transform.position = CollisionUtils.AdjustPositionLeft(leftCollidedInfo, _player);
});
    }
  }
  public override void Exit()
  {
    base.Exit();
  }

}
