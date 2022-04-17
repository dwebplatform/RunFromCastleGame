using UnityEngine;
public class JumpingState : BaseState
{
    private float _movementSpeed = 5f;
    private PlayerController _playerController;
    private Player _player;
    private float _jumpForce = 5f;
    private float _startTime;
    private float _offset = 0.1f;
    private bool _isGrounded;
    HittedParams hittedParams;
    public JumpingState(string name, PlayerController playerController, Player player): base(name){
        _playerController = playerController;
        _player = player;
    }
    private bool IsGravityIgnored(){
        float delta = Time.time - _startTime;
        if(delta<0.2f){
            return true;
        } 
        return false;
    }
  public override void Enter()
  {
    base.Enter();
    _player.velocity = new Vector2(_player.velocity.x, _jumpForce);
    _isGrounded = false;
    _startTime = Time.time;
  }

  public override void LogicUpdate()
  {
    base.LogicUpdate();
    bool isGravityIgnored = IsGravityIgnored();
    if(_isGrounded && !isGravityIgnored){
      _playerController.ChangeState(PlayerController.idleState);
    }
    if(_player.closestWall != null){
        //* если давит в эту сторону, то в grabbing state
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
  }

  public override void PhysicsUpdate()
  {
    base.PhysicsUpdate();
    bool isGravityIgnored = IsGravityIgnored();
    _player.velocity = new Vector2(PlayerController.horizontalInput*_movementSpeed,_player.velocity.y);
    _playerController.collisionManager.CheckGround(_offset, (surfacePoint, boxSize) =>
    {
      _isGrounded = true;
      if(!isGravityIgnored){
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
    _player.velocity.x = 0f;
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
    _player.velocity.x = 0f;

  });

    if(!hittedParams.isHittedLeft && !hittedParams.isHittedRight){
      _player.closestWall = null;
    }
    if(!_isGrounded){
        _player.velocity.y-=Player.GRAVITY*Time.fixedDeltaTime;
    }

  }
}
