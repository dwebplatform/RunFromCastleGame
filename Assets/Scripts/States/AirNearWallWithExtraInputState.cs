using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirNearWallWithExtraInputState : BaseState
{
  private PlayerController _playerController;
  private Player _player;

  private bool _isExtraInputFinished;
  private float _offset = 0.1f;
  private bool _isGrounded;

  private float _startTime;

  private HittedParams hittedParams;
  public AirNearWallWithExtraInputState(string name, PlayerController playerController, Player player) : base(name)
  {
    _player = player;
    _playerController = playerController;
  }
  public override void Enter()
  {
    base.Enter();
    _player.velocity = Vector2.zero;
    _player.velocity.y = 5f;
    _isExtraInputFinished = false;
    _isGrounded = false;
    _startTime = Time.time;

  }


  private bool IsGravityIgnored()
  {
    float delta = Time.time - _startTime;
    if (delta < 0.2f)
    {
      return true;
    }
    return false;
  }
  private bool IsTryMoveAwayFromWall()
  {
    if(_player.closestWall==null){
      return false;
    }
    return (_player.closestWall.normal.x > 0f && PlayerController.horizontalInput > 0f) || (_player.closestWall.normal.x < 0f && PlayerController.horizontalInput < 0f);
  }
  private bool IsPressedMovementButtons()
  {
    return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow);
  }
  public override void Exit()
  {
    base.Exit();
    _isGrounded = false;
  }
  public override void LogicUpdate()
  {
    base.LogicUpdate();
    if (_isGrounded && !IsGravityIgnored())
    {
      _playerController.ChangeState(PlayerController.idleState);
    }
    if (Input.GetKeyDown(KeyCode.Space) && !IsGravityIgnored())
    {
      _playerController.ChangeState(PlayerController.recoilFromWallWithHorizontalEffectState);
    }
    //* пока не обнулился приденный инпут, игнорируем перемещение
    if (!_isExtraInputFinished)
    {
      if (IsPressedMovementButtons())
      {
        _isExtraInputFinished = true;
      }
      else
      {
        if (PlayerController.horizontalInput < Mathf.Epsilon)
        {
          _isExtraInputFinished = true;
        }
      }
    }
    else
    {
      if (IsTryMoveAwayFromWall())
      {
        _playerController.ChangeState(PlayerController.fallingFreeFromWallState);
      }
    }
  }

  public override void PhysicsUpdate()
  {
    base.PhysicsUpdate();
    bool isGravityIgnored = IsGravityIgnored();
    _playerController.collisionManager.CheckGround(_offset, (surfacePoint, boxSize) =>
    {
      _isGrounded = true;
      if (!isGravityIgnored)
      {

        _player.transform.position = new Vector3(_player.transform.position.x,
        surfacePoint.y + boxSize.y / 2, _player.transform.position.z);
        _player.velocity.y = 0f;
      }
    }, () =>
    {
      _isGrounded = false;
    }).WallCheck(Player.WALL_OFFSET,
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

    if (!_isGrounded && !isGravityIgnored)
    {
      _player.velocity.y -= Player.GRAVITY * Time.fixedDeltaTime;
    }
  }
}
