using UnityEngine;

public class WalkingState : GroundedState
{
  private PlayerController _playerController;
  private Player _player;
  private float _movementSpeed = 5f;
  HittedParams hittedParams;
  public WalkingState(string name, PlayerController playerController, Player player) : base(name, playerController, player)
  {
    _player = player;
    _playerController = playerController;
  }

  public override void Enter()
  {
    base.Enter();
  }
  public override void HandleInput()
  {
    base.HandleInput();
    if (Input.GetKeyDown(KeyCode.Space))
    {
      if (_player.closestWall != null)
      {
        _playerController.ChangeState(PlayerController.airNearWallState);
      }
      else
      {
        _playerController.ChangeState(PlayerController.jumpingState);
      }
    }
  }
  public override void LogicUpdate()
  {
    base.LogicUpdate();
    if (Mathf.Abs(PlayerController.horizontalInput) < Mathf.Epsilon)
    {
      _playerController.ChangeState(PlayerController.idleState);
    }
  }
  public override void PhysicsUpdate()
  {
    base.PhysicsUpdate();
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
});
    if (!hittedParams.isHittedLeft && !hittedParams.isHittedRight)
    {
      _player.closestWall = null;
    }

    if (_player.closestWall != null)
    {
      Vector2 normal = _player.closestWall.normal;
      if (CollisionUtils.IsWallInFront(normal, PlayerController.horizontalInput))
      {
        _player.transform.position = CollisionUtils.GetAdjustedPosition(_player);
      }
      _player.velocity = GetVelocityInfluencedByWall(_player.closestWall);
    }
    else
    {
      _player.velocity = new Vector2(PlayerController.horizontalInput * _movementSpeed, _player.velocity.y);
    }
  }
  private Vector2 GetVelocityInfluencedByWall(WallInfo closestWall)
  {
    if (Mathf.Abs(PlayerController.horizontalInput) > 0f)
    {
      Vector2 normal = closestWall.normal;
      if (CollisionUtils.IsWallInFront(normal, PlayerController.horizontalInput))
      {
        return new Vector2(0, _player.velocity.y);
      }
      else
      {
        return new Vector2(PlayerController.horizontalInput * _movementSpeed, _player.velocity.y);
      }
    }
    return new Vector2(0f, _player.velocity.y);
  }
}
