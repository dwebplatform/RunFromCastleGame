using UnityEngine;
public class GrabbingWallState : BaseState
{
  private PlayerController _playerController;

  private float _offset = 0.1f;
  private Player _player;
  private bool _isGrounded;
  HittedParams hittedParams;
  public GrabbingWallState(string name, PlayerController playerController, Player player) : base(name)
  {
    _player = player;
    _playerController = playerController;
  }
  public override void Enter()
  {
    base.Enter();
    _player.velocity = Vector2.zero;
    _isGrounded = false;
  }
  public override void LogicUpdate()
  {
    base.LogicUpdate();
    if (_isGrounded)
    {
      _playerController.ChangeState(PlayerController.idleState);
    }

    if (_player.closestWall != null)
    {
      Vector2 normal = _player.closestWall.normal;
      if (Mathf.Abs(PlayerController.horizontalInput) > 0f)
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
    }
  public override void Exit()
  {
    base.Exit();
  }
  public override void PhysicsUpdate()
  {
    base.PhysicsUpdate();
    _player.velocity = new Vector2(0f, _player.velocity.y);
    _playerController.collisionManager.CheckGround(_offset, (surfacePoint, boxSize) =>
    {
      _isGrounded = true;
      _player.transform.position = new Vector3(_player.transform.position.x,
      surfacePoint.y + boxSize.y / 2, _player.transform.position.z);
      _player.velocity.y = 0f;
    }, () =>
    {
      _isGrounded = false;
    })

.WallCheck(Player.WALL_OFFSET,
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
    if (!_isGrounded)
    {
      _player.velocity.y -= (Player.GRAVITY / 2) * Time.fixedDeltaTime;
    }
  }
}
