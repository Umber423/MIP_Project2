using UnityEngine;

public class Projectile2D : MonoBehaviour
{
    [Header("Physics")]
    public float gravityScale = 6f;
    public float mass = 3f;        // HEAVY
    public float drag = 2.5f;      // SLOWS MOVEMENT

    [Header("Despawn")]
    public float lifeTime = 4f;
    public LayerMask groundLayer;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        rb.mass = mass;
        rb.drag = drag;

        Destroy(gameObject, lifeTime);

        // Ignore player collision
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            Collider2D pc = player.GetComponent<Collider2D>();
            Collider2D bc = GetComponent<Collider2D>();
            if (pc && bc)
                Physics2D.IgnoreCollision(bc, pc);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (((1 << col.gameObject.layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}
