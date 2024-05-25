using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBorder : MonoBehaviour
{

    public bool isLeft = true;
    private BoxCollider2D collider;

    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }
    void Start()
    {
        //TODO 处理一些分辨率的适配    
        // 获取摄像机的边界位置
        float cameraHeight = 2f * Camera.main.orthographicSize;
        float cameraWidth = cameraHeight * Camera.main.aspect;
        Debug.Log("camaer wid: " + cameraWidth + "   collider:" + collider.size.x);
        var pos = transform.position;
        if (isLeft)
        {
            pos.x = -cameraWidth / 2 - collider.size.x / 2;
        }
        else
        {
            pos.x = cameraWidth / 2 + collider.size.x / 2;
        }
        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance.CurState == GameManager.GameState.Running && other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.PlayerDie();
        }
    }
}
