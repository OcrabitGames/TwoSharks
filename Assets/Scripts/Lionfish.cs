using UnityEngine;

public class Lionfish : MonoBehaviour
{
    public float speed = 3f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * (speed * Time.deltaTime));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var collidedObject =  collision.gameObject;
        if (collidedObject.CompareTag("Shark"))
        {
            Shark sharkScript = collidedObject.GetComponent<Shark>();
            sharkScript.TriggerGameOver();
        }
    }
}
