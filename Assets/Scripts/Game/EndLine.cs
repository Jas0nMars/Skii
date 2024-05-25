using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLine : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem[] finishEffect;
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer.sprite;
        float width = sprite.rect.width / sprite.pixelsPerUnit;
        float height = sprite.rect.height / sprite.pixelsPerUnit;

        Vector3 localScale = transform.localScale;
        float actualWidth = width * localScale.x;
        float actualHeight = height * localScale.y;
        Debug.Log("Actual Sprite Width: " + actualWidth + " units");
        Debug.Log("Actual Sprite Height: " + actualHeight + " units");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        foreach(ParticleSystem fx in finishEffect)
        {
            fx.gameObject.SetActive(true);
            fx.Play();
        }
        GameManager.Instance.GameFinish();   
        Debug.Log("EndLine: " + other.transform.position.y);
    }

    public void Appear(Vector3 pos)
    {
        transform.position = pos;
         foreach(ParticleSystem fx in finishEffect)
        {
            fx.gameObject.SetActive(false);
            fx.Play();
        }
    }
}
