using UnityEngine;

public class Player 
{
    public Vector2 velocity;

    public static float GRAVITY = 5f;
    public static float WALL_OFFSET = 0.1f;
    public BoxCollider2D boxCollider;
    public Transform transform;
    public WallInfo closestWall;
    public Player(BoxCollider2D boxCollider, Transform transform ){
        this.boxCollider = boxCollider;
        this.transform = transform;
    }
    public Vector2 addToVelocity(Vector2 delta){
      velocity += delta;
      return velocity;    
    }
    public void NormalizeVelocity(){
        velocity = velocity.normalized;
    }

    public Vector2 removeFromVelocity(Vector2 delta){
        velocity -= delta;
        return velocity;
    }
}
