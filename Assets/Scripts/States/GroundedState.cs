using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : BaseState
{
    private PlayerController _playerController;
    private float _offset = 0.1f;
    private Player _player;
    protected bool _isGrounded;
    public GroundedState(string name,PlayerController playerController, Player player ):base(name){ 
        _playerController = playerController;
        _player = player;
    }
  public override void LogicUpdate()
  {
      base.LogicUpdate();
  }
  public override void Exit()
  {
    base.Exit();
  }
  public override void Enter()
  {
    base.Enter();
  }
  public override void HandleInput()
  {
    base.HandleInput();
  }
  public override void PhysicsUpdate()
  {
    base.PhysicsUpdate();
    _playerController.collisionManager.CheckGround(_offset,(surfacePoint, boxSize)=>{
      _isGrounded = true;
      
      _player.transform.position = new Vector3(_player.transform.position.x,
      surfacePoint.y + boxSize.y / 2, _player.transform.position.z);
      _player.velocity.y = 0f;
    },()=>{
        _isGrounded = false;
    });
    if(!_isGrounded){
        _player.velocity.y -= Player.GRAVITY*Time.fixedDeltaTime;
    }
  }
}
