using UnityEngine;
using System;
public class CollisionManager
{
  private Player _player;
  private LayerMask isWallLayer;
  private ContactFilter2D _filter;
  private Collider2D[] results = new Collider2D[1];
  public CollisionManager(Player player)
  {
    _player = player;
    
    _filter.SetLayerMask(LayerMask.GetMask("Ground"));
    isWallLayer = LayerMask.GetMask("Wall");
  }
  public CollisionManager CheckGround(float offset,
  Action<Vector2, Vector2> groundedCallBack,
  Action notGroundedCallBack)
  {
    Vector2 boxSize = new Vector2(_player.transform.localScale.x,
    _player.transform.localScale.y);
    Vector2 point = _player.transform.position + Vector3.down * offset;
    if (Physics2D.OverlapBox(point, boxSize, 0, _filter, results) > 0)
    {
      Vector2 surfacePosition = Physics2D.ClosestPoint(_player.transform.position, results[0]);
      groundedCallBack(surfacePosition, boxSize);
    }
    else
    {
      notGroundedCallBack();
    }
    return this;
  }

  public CollisionManager WallCheck(float offset, Action<CollidedInfo, bool> onHitRightWallCallBack,
  Action<CollidedInfo, bool> onHitLeftWallCallBack)
  {
    RaycastHit2D rightHit = Physics2D.Raycast(_player.transform.position,
    Vector2.right, _player.boxCollider.size.y / 2 + offset,
    isWallLayer);

    RaycastHit2D leftHit = Physics2D.Raycast(_player.transform.position,
    -Vector2.right,
    _player.boxCollider.size.y / 2 + offset,
    isWallLayer);

    CollidedInfo rightCollidedInfo = new CollidedInfo();
    CollidedInfo leftCollidedInfo = new CollidedInfo();
    if (rightHit.collider != null)
    {
      rightCollidedInfo.normal = rightHit.normal;
      rightCollidedInfo.collider = rightHit.collider;
      rightCollidedInfo.wallType = "RIGHT";
      onHitRightWallCallBack(rightCollidedInfo, true);
    }
    else
    {
      onHitRightWallCallBack(rightCollidedInfo, false);
    }

    if (leftHit.collider != null)
    {
      leftCollidedInfo.normal = leftHit.normal;
      leftCollidedInfo.collider = leftHit.collider;
      leftCollidedInfo.wallType = "LEFT";
      Vector3 normal = leftCollidedInfo.normal;

      onHitLeftWallCallBack(leftCollidedInfo, true);
    }
    else
    {
      onHitLeftWallCallBack(leftCollidedInfo, false);
    }

    return this;
  }
}
