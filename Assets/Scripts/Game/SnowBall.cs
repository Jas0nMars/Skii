using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBall : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public SpriteRenderer body;
    public SpriteRenderer leftMovingShadwSpr;
    public SpriteRenderer rightMovingShadwSpr;
    public TrailRenderer trail;
    // Start is called before the first frame update
    public bool canMove = false;

    public Vector3 startPos;
    public Vector3 endPos;

    public float MovingTime = 2f;
    private float currentMovingTime = 0f;
    public bool isMovingRight = false;

    private float cameraHeight;
    private float cameraWidth;

    public float offsetX = 1;
    void Awake()
    {
        cameraHeight = 2f * Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Camera.main.aspect;
    }

    // Update is called once per frame
    void Update()
    {
        body.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        if (!canMove) return;
        currentMovingTime += Time.deltaTime;
        transform.position = Vector3.Lerp(startPos, endPos , currentMovingTime / MovingTime);
    }

    //屏幕上半区域生成， 向对角方向移动
    public void ShowSnowBall()
    {
        bool isMovingRight = Random.Range(0, 2) == 0;
        leftMovingShadwSpr.gameObject.SetActive(!isMovingRight);
        rightMovingShadwSpr.gameObject.SetActive(isMovingRight);
        float range = cameraHeight / 2f;
        // Debug.Log("heirght: " + cameraHeight +"range: " + range);
        float startPosX = isMovingRight ? -cameraWidth / 2f - offsetX : cameraWidth / 2f + offsetX;
        float startPosY = Camera.main.transform.position.y + Random.Range(0f, range);
        float endPosX = !isMovingRight ? -cameraWidth / 2f - offsetX : cameraWidth / 2f + offsetX;
        float endPosY = Camera.main.transform.position.y - Random.Range(0f, range);
        startPos = new Vector3(startPosX, startPosY, 0);
        endPos = new Vector3(endPosX, endPosY, 0);
        transform.position = new Vector3(startPosX, startPosY, 0);
        canMove = true;
        //10s自动销毁 简单暴力
        Destroy(this, 10);
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        GameManager.Instance.PlayerDie();
    }

    public void ClearEff()
    {
        trail.Clear();
    }
}
