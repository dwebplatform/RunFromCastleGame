using UnityEngine;

public class RecoilFromWallWithHorizontalEffectState : BaseState
{
    private PlayerController _playerController;
    private Player _player;
    HittedParams hittedParams;
    public RecoilFromWallWithHorizontalEffectState(string name,PlayerController playerController, Player player):base(name){
        _player = player;
        _playerController = playerController;

    }
 
  private bool isHorizontalInfluencePresent;

  private float _offset = 0.1f;
  private bool _isGrounded;
  private Vector2 _initialVelocity;
  private float _startTime;
  public override void Enter()
  {
    base.Enter();
    isHorizontalInfluencePresent = true;

    _initialVelocity = new Vector2(
        _player.closestWall.normal.x,
        _player.closestWall.normal.y).normalized;
        _initialVelocity.x*=5f;
        _initialVelocity.y = 5f;
        _player.velocity = _initialVelocity;
        _player.closestWall = null;

    _startTime = Time.time;
  }

  private bool IsWallCheckIgnored(){
      float delta = Time.time - _startTime;
      if(delta<0.2f){
          return true;
      }
      return false;
  }
  private bool IsPlayerPressedAnyMovement(){
      return Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.D)||Input.GetKeyDown(KeyCode.LeftArrow)||Input.GetKeyDown(KeyCode.RightArrow);
  }

  private bool IsDirectionSameAsInput(){
      return PlayerController.horizontalInput > 0f && _player.velocity.x > 0f || PlayerController.horizontalInput<0f && _player.velocity.x<0f;
  }
  public override void LogicUpdate()
  {
    base.LogicUpdate();
    if(_isGrounded){
        _playerController.ChangeState(PlayerController.idleState);
    }

    if(isHorizontalInfluencePresent){

        if(IsPlayerPressedAnyMovement()){
            if(IsDirectionSameAsInput()){
                //* просто идет также, если наоборот, то перешел в walking state
            } else {
                if(Mathf.Abs(PlayerController.horizontalInput)>Mathf.Epsilon){
                    _playerController.ChangeState(PlayerController.walkingState);
                }
            }
        } else{
            //* если ничего не нажал, и скорость обнулилась, то влияние предыдущего инпута закончилось
            if(Mathf.Abs(PlayerController.horizontalInput)<Mathf.Epsilon){
                //* просто идет также, если наоборот, то перешел в walking state
                isHorizontalInfluencePresent = false;
            }
        }
    }
    if(!isHorizontalInfluencePresent){
        //* если время игноринга стены закончилось, значит персонаж может реагировать на стенку
        if(!IsWallCheckIgnored()){
            if(_player.closestWall!=null){
                _playerController.ChangeState(PlayerController.grabbingWallState);
            }
        }
        if(Mathf.Abs(PlayerController.horizontalInput)>Mathf.Epsilon){
            //* нажал туда же продолжаем движение, нажал обратно freeFromWallState
            if(IsDirectionSameAsInput()){
                //* если скорость  и инпут идут туда же, то просто продолжаем движение
            } else {
                _playerController.ChangeState(PlayerController.walkingState);
            }
        }
    } else{

    }

  }

  public override void PhysicsUpdate()
  {
    base.PhysicsUpdate();
    _playerController.collisionManager.CheckGround(_offset, (surfacePoint, boxSize) =>
    {
      _isGrounded = true;
    
    }, () =>
    {
      _isGrounded = false;
    }).WallCheck(Player.WALL_OFFSET,
    (rightCollider, isHitted) =>
  {
    hittedParams.isHittedRight = isHitted;
    if (!isHitted||IsWallCheckIgnored())
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
    if (!isHitted || IsWallCheckIgnored())
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

    if(!_isGrounded){
        _player.velocity.y-=Player.GRAVITY*Time.fixedDeltaTime;
    }
  }

}
