using UnityEngine;
public class FallingFreeFromWallState : GroundedState
{
  private PlayerController _playerController;
  private Player _player;
  float _startTime;
  private HittedParams hittedParams;
  private float _movementSpeed = 5f;
  public FallingFreeFromWallState(string name, PlayerController playerController, Player player) : base(name, playerController, player)
  {
    _playerController = playerController;
    _player = player;
  }
  private bool IsWallCheckIgnored()
  {
    float delta = Time.time - _startTime;
    if (delta < 0.2)
    {
      return true;
    }
    return false;
  }
  public override void Enter()
  {
    base.Enter();
    _startTime = Time.time;
    _isGrounded = false;
  }
  public override void HandleInput()
  {
    base.HandleInput();
  }

  public override void LogicUpdate()
  {
    base.LogicUpdate();
    bool isWallCheckIgnored = IsWallCheckIgnored();
        
    if(_isGrounded && !isWallCheckIgnored){
       _playerController.ChangeState(PlayerController.idleState);   
    }

    if(_player.closestWall!=null){
        Vector2 normal = _player.closestWall.normal;
        if(Mathf.Abs(PlayerController.horizontalInput)>0f){
        if(CollisionUtils.IsWallInFront(normal, PlayerController.horizontalInput)){
            //* если стена РЕАЛЬНО ПЕРЕД ПЕРСОНАЖЕМ
            //* если faceнулся со стенкой при передвижении, сразу переходишь в grabbingWallState
            _playerController.ChangeState(PlayerController.grabbingWallState);
        } else {
         
        }
        }
    }
    if(Mathf.Abs(PlayerController.horizontalInput)>Mathf.Epsilon){
        _player.velocity = new Vector2(PlayerController.horizontalInput*_movementSpeed, _player.velocity.y);
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
  if(!hittedParams.isHittedLeft && !hittedParams.isHittedRight){
    _player.closestWall = null;
  } 
  }
  public override void Exit()
  {
    base.Exit();
  }
}
