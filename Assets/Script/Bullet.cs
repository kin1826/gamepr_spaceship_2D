using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    
    public GameObject hitEffectPrefab;
    
    void Awake()
    {
        int levelIndex = Mathf.Clamp(GameManager.instance.currentLevel - 1, 0, GameConfig.Bullet.damage.Length - 1);
        damage = GameConfig.Bullet.damage[levelIndex];
        Debug.Log("Current Bullet Damage: " + damage);
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Asteroid"))
        {
            var asteroid = collision.GetComponent<Asteroid>();

            if (asteroid != null)
            {
                asteroid.TakeDamage(damage);
            }
            
            //xử lý hit effect - Xoay theo hướng mà đạn bắn ra
            GameObject hit = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            hit.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            //audio hit - no
            // AudioManager.instance.PlaySFX(AudioManager.instance.hitSound);
            
            Destroy(gameObject);
        }
    }
}
