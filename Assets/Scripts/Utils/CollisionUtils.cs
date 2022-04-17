using UnityEngine;


public class CollisionUtils
{
  static public bool IsWallInFront(Vector2 normal, float horizontalInput)
  {
    return (normal.x > 0f && horizontalInput <0f) || (normal.x < 0f && horizontalInput > 0f);
  }
  static public Vector3 AdjustPositionRight(CollidedInfo collidedInfo, 
  Player player)
  {
    Vector3 normal = collidedInfo.normal.normalized;
    Vector3 difference = collidedInfo.collider.transform.position - 
    player.transform.position;
    float gap = difference.x - collidedInfo.collider.bounds.size.x / 2 - player.boxCollider.size.x / 2;
    Vector3 correctedPosition = player.transform.position - new Vector3(normal.x * gap, normal.y * gap, 0f);
    return correctedPosition;
  }

  static public Vector3 GetAdjustedPosition(Player player)
  {
    CollidedInfo colliderInfo = new CollidedInfo();
    colliderInfo.normal = player.closestWall.normal;
    colliderInfo.collider = player.closestWall.collider;
    colliderInfo.wallType = player.closestWall.normal.x > 0 ? "LEFT" : "RIGHT";
    if (colliderInfo.wallType == "LEFT")
    {
      return CollisionUtils.AdjustPositionLeft(colliderInfo, player);
    }
    else
    {
      return CollisionUtils.AdjustPositionRight(colliderInfo, player);
    }
  }
  static public Vector3 AdjustPositionLeft(CollidedInfo collidedInfo, Player player)
  {
    Vector3 normal = collidedInfo.normal.normalized;
    Vector3 difference = player.transform.position - collidedInfo.collider.transform.position;
    float gap = difference.x - collidedInfo.collider.bounds.size.x / 2 - player.boxCollider.bounds.size.x / 2;
    Vector3 newPosition = player.transform.position - new Vector3(normal.x * gap, normal.y * gap, 0f);
    return newPosition;
  }

}
