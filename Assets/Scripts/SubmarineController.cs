using UnityEngine;

public class SubmarineController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    public GameObject torpedoPrefab;
    public Transform firePoint;

    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        FlipSprite();

        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement.normalized * moveSpeed;
    }

    void FlipSprite()
    {
        if (movement.x < 0)
            transform.localScale = new Vector3(-3, 3, 3);
        else if (movement.x > 0)
            transform.localScale = new Vector3(3, 3, 3);
    }

    void Shoot()
    {
        Instantiate(torpedoPrefab, firePoint.position, firePoint.rotation);
    }
}
