
using System.Data;
using UnityEngine;

public class Health : MonoBehaviour
{


    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    //private Animator anim;
    bool dead = false;


    private void Awake()
    {
        currentHealth = startingHealth;
        //anim = GetComponent<Animator>();
        //spriteRend = GetComponent<SpriteRenderer>();
    }
    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            //anim.SetTrigger("hurt");

        }
    }


    public void Update()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null && dead == true)
        {
            boxCollider.enabled = false;
            gameObject.SetActive(false);
            dead = true;

        }
    }

}


