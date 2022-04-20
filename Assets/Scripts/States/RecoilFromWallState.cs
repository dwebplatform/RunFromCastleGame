// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class RecoilFromWallState : BaseState
{
  private PlayerController _playerController;
  private Player _player;

  private float _offset = 0.1f;

  private bool _isGrounded;

  private HittedParams hittedParams;

  private float _startTime;
  public RecoilFromWallState(string name, PlayerController playerController, Player player) : base(name)
  {
    _playerController = playerController;
    _player = player;
  }
  public override void Enter()
  {
    base.Enter();
    _isGrounded = false;
    _startTime = Time.time;
    _player.velocity = new Vector2(_player.closestWall.normal.x * 5f, 5f);
    _player.closestWall = null;
  }

  private bool IsWallIgnored()
  {
    float delta = Time.time - _startTime;
    if (delta < 0.2f)
    {
      return true;
    }
    return false;
  }

  private bool IsPressedReversedMovement(){
    return (PlayerController.horizontalInput>0f && _player.velocity.x<0f)||(PlayerController.horizontalInput<0f && _player.velocity.x>0f);
  }
  public override void LogicUpdate()
  {
    base.LogicUpdate();
    if (_isGrounded)
    {
      _playerController.ChangeState(PlayerController.idleState);
    }
    if(!IsWallIgnored()){
      if(_player.closestWall!=null){
        _playerController.ChangeState(PlayerController.grabbingWallState);
      } else {
        //* если нажал на кнопку противоположную текущему движению
        if(IsPressedReversedMovement()){
          _playerController.ChangeState(PlayerController.walkingState);
        }
      }
    }

  }
  public override void PhysicsUpdate()
  {
    base.PhysicsUpdate();
    _playerController.collisionManager.CheckGround(_offset, (surfacePoint, boxSize) =>
    {
      _isGrounded = true;
      _player.transform.position = new Vector3(_player.transform.position.x,
      surfacePoint.y + boxSize.y / 2, _player.transform.position.z);
      _player.velocity.y = 0f;
    }, () =>
    {
      _isGrounded = false;
    }).WallCheck(Player.WALL_OFFSET,
    (rightCollider, isHitted) =>
  {
    hittedParams.isHittedRight = isHitted;
    if (!isHitted || IsWallIgnored())
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
    if (!isHitted || IsWallIgnored())
    {
      return;
    }
    _player.closestWall = new WallInfo();
    _player.closestWall.collider = leftCollidedInfo.collider;
    _player.closestWall.normal = leftCollidedInfo.normal;
    _player.transform.position = CollisionUtils.AdjustPositionLeft(leftCollidedInfo, _player);
  });
    if (!(hittedParams.isHittedLeft || hittedParams.isHittedRight))
    {
      _player.closestWall = null;
    }
    if (!_isGrounded)
    {
      _player.velocity.y -= Player.GRAVITY * Time.fixedDeltaTime;
    }
  }
}
