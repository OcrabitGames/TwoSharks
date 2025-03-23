using NUnit.Framework.Constraints;
using UnityEngine;

public class Lionfish : MonoBehaviour
{
    public float speed = 3f;
    private bool _isActive = true;
    
    public void Activate()
    {
        _isActive = true;
        gameObject.SetActive(true);
    }
    
    public void Deactivate()
    {
        _isActive = false;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isActive) return;
        transform.Translate(Vector3.down * (speed * Time.deltaTime));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var collidedObject =  collision.gameObject;
        if (collidedObject.CompareTag("Shark"))
        {
            Shark sharkScript = collidedObject.GetComponent<Shark>();
            sharkScript.TriggerGameOver();
        }
    }

    public void StopMovement()
    {
        _isActive = false;
    }
}
