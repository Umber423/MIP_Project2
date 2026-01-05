using UnityEngine;

public class ObjectShooter2D : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootForce = 20f; // DEFAULT = 20

    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        AimAtMouse();

        if (Input.GetMouseButtonDown(0))
            Shoot();
    }

    void AimAtMouse()
    {
        Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;

        Vector2 dir = (mouse - firePoint.position).normalized;
        firePoint.right = dir;
    }

    void Shoot()
    {
        GameObject proj = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.AddForce(firePoint.right * shootForce, ForceMode2D.Impulse);
        }
    }
}
