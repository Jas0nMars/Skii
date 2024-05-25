using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private float offsetY;
    // Update is called once per frame
    private void Awake() {
        offsetY = transform.position.y - GameManager.Instance.player.transform.position.y;
    }
    void Update()
    {
        if(GameManager.Instance.CurState == GameManager.GameState.Running){
            Vector3 tempPos = transform.position;
            tempPos.y -= Time.deltaTime * Config.VerticalMoveSpeed;
            transform.position = tempPos;
        }
    }
}
