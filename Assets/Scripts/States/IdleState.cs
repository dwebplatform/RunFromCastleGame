using UnityEngine;
using System;

public class WallInfo {
  public Collider2D collider;
  public Vector2 normal;
}
public class IdleState : GroundedState
{
  private HittedParams hittedParams;
  private PlayerController _playerController;
  private Player _player;
  public WallInfo wallInfo = null;
  public IdleState(string name, PlayerController playerController, Player player) : 
  base(name, playerController, player)
  {
    _player = player;
    _playerController = playerController;
  }

  public override void Enter()
  {
    base.Enter();
    _player.velocity = new Vector2(0f, _player.velocity.y);
  }
  public override void HandleInput()
  {
    base.HandleInput();
  }
  private bool _isWallInFront;
  public override void LogicUpdate()
  {
    base.LogicUpdate();
     
    if(Input.GetKey(KeyCode.Space)){
      if(_player.closestWall != null){
          // _player.closestWall = wallInfo;
          _playerController.ChangeState(PlayerController.airNearWallState);
      } else {
        _playerController.ChangeState(PlayerController.jumpingState);
      }
    }
    if (Mathf.Abs(PlayerController.horizontalInput) > Mathf.Epsilon)
    {
      _playerController.ChangeState(PlayerController.walkingState);
    }
  }
  public override void PhysicsUpdate()
  {
    base.PhysicsUpdate();
    _playerController.collisionManager.WallCheck(Player.WALL_OFFSET, 
    (rightCollider,isHitted) =>
  {
    hittedParams.isHittedRight  = isHitted; 
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
  if(!(hittedParams.isHittedLeft||hittedParams.isHittedRight)){
    _player.closestWall = null;
  }
  }
}
