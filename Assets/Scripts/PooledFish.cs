using UnityEngine;
using UnityEngine.Serialization;

public class PooledFish : MonoBehaviour
{
    private float _speed;
    [FormerlySerializedAs("screenCutOff")] public float initialScreenCutOff;
    private bool _isActive;
    public bool isBlue;
    public bool isMinnow;
    
    public void Activate(Vector3 position, float speed)
    {
        transform.position = position;
        _speed = speed;
        _isActive = true;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        _isActive = false;
        gameObject.SetActive(false);
    }

    public void Freeze()
    {
        _isActive = false;
    }

    public void Unfreeze()
    {
        _isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isActive) return;
        
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));

        if (transform.position.y < -initialScreenCutOff)
        {
            if (isMinnow)
            {
                GameManager.Instance.GameOver();
            }
            else
            {
                FishManager.Instance.ReturnFish(this);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.gameObject.CompareTag("Shark")) return;
        
        if (isMinnow)
        {
            FishManager.Instance.ReturnFish(this);
            GameManager.Instance.IncreaseScore();
        }
        else
        {
            GameManager.Instance.GameOver();
        }

    }
}
