using UnityEngine;

public class Ennemi_movement : MonoBehaviour
{
    public float speed;
    private bool isChasing;

    private EnemyState enemyState;

    private Rigidbody2D rb;
    private Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //////////ChangeState(enemyState.Idle);
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isChasing == true)
        {
            
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")   
        {
            player = collision.transform;
            isChasing = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rb.linearVelocity = Vector2.zero;
            isChasing = false;
        }
    }

    void ChangeState(EnemyState newState)
    {
        enemyState = newState;
    }

}
public enum EnemyState
{
    Idle,
    Chasing, 
}
