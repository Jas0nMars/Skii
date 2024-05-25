using System;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class Player : MonoBehaviour
    {
        public float vSpeed;
        public float hMaxSpeed;
        public float hAccSpeed;
        public float changeDirTime = 0.1f;
        private float curChangeDirTime = 0f;
        private float curDeAccTime = 0f;
        public float deAccSpeedTime = 0.2f;
        public float curHSpeed = 0f;
        public float deAccOriginHSpeed = 0f;
        private float changDirOriginSpeed = 0f;

        public bool isMovingRight = true;
        public bool isAccelerate = false;

        private TrailRenderer trail;
        public SpriteRenderer body;
        public SpriteRenderer hand;
        public SpriteRenderer foot;
        public SpriteRenderer face;

        public ParticleSystem deathFx;
        public GameObject bodyRoot;
        private float finishFreeMoveTime = 0f;

        private float originPosY = 0f;



        private void Awake()
        {
            //初始化一致，防止上来就转弯
            curChangeDirTime = changeDirTime;
            originPosY = transform.position.y;
            trail = GetComponent<TrailRenderer>();
        }

        private void Update()
        {
            if (GameManager.Instance.CurState == GameManager.GameState.Running)
            {
                Vector3 temp = transform.position;
                if (curChangeDirTime <= changeDirTime)
                {
                    curChangeDirTime += Time.deltaTime;
                    if (isMovingRight)
                    {
                        curHSpeed = Mathf.Lerp(changDirOriginSpeed, isAccelerate ? hMaxSpeed + hAccSpeed : hMaxSpeed, curChangeDirTime / changeDirTime);
                    }
                    else
                    {
                        curHSpeed = Mathf.Lerp(changDirOriginSpeed, isAccelerate ? -(hMaxSpeed + hAccSpeed) : -hMaxSpeed, curChangeDirTime / changeDirTime);
                    }
                }
                else
                {
                    //长按后的减速过程
                    if (Mathf.Abs(curHSpeed) > hMaxSpeed && !isAccelerate)
                    {
                        curDeAccTime += Time.deltaTime;
                        curHSpeed = Mathf.Lerp(deAccOriginHSpeed, isMovingRight ? hMaxSpeed : -hMaxSpeed, curDeAccTime / deAccSpeedTime);
                    }
                }
                temp.x += curHSpeed * Time.deltaTime;
                temp.y -= vSpeed * Time.deltaTime;

                transform.position = temp;
                GameManager.Instance.UpdateProgress(temp.y);
            }
            else if (GameManager.Instance.CurState == GameManager.GameState.GameEnd)
            {
                //左右方向随机 不能控制
                Vector3 temp = transform.position;
                curHSpeed = Mathf.Lerp(finishLineOriginSpeed, -(finishLineOriginSpeed + hAccSpeed), finishFreeMoveTime); // 1s变化，2s切换关卡， 就变换一次，感觉也还可以
                temp.x += curHSpeed * Time.deltaTime;
                temp.y -= vSpeed * Time.deltaTime;

                transform.position = temp;
                finishFreeMoveTime += Time.deltaTime;
            }
        }
        public void ChangeDir()
        {
            isMovingRight = !isMovingRight;
            changDirOriginSpeed = curHSpeed;
            curChangeDirTime = 0f;
            ChangeDirSpr();
        }

        private void ChangeDirSpr()
        {
            var tempAngles = face.transform.localEulerAngles;
            if (isMovingRight)
            {
                tempAngles.y = 0;
                foot.transform.localEulerAngles = tempAngles;
                hand.transform.localEulerAngles = tempAngles;
            }
            else
            {
                tempAngles.y = 180;
                foot.transform.localEulerAngles = tempAngles;
                hand.transform.localEulerAngles = tempAngles;
            }
        }

        public void StartAccelerate()
        {
            isAccelerate = true;
        }

        public void StopAccelerate()
        {
            isAccelerate = false;
            // curChangeDirTime = changeDirTime;
            curDeAccTime = 0;
            deAccOriginHSpeed = curHSpeed;
        }

        public void ResetPos()
        {
            var temp = transform.position;
            temp.x = 0;
            transform.position = temp;
        }

        public void SetThemeColor()
        {
            trail.startColor = GameManager.Instance.CurThemeColor;
            trail.endColor = Color.white;
            body.color = GameManager.Instance.CurThemeColor;
            var main = deathFx.main;
            // 设置新的 StartColor
            main.startColor = GameManager.Instance.CurThemeColor;
        }

        public void ResetState(bool resetToBornPos = true)
        {
            if (resetToBornPos)
            {
                var temp = transform.position;
                temp.y = originPosY;
                transform.position = temp;
            }
            else
            {
                var temp = transform.position;
                temp.x = 0;
                transform.position = temp;
            }
            trail.Clear();
            isAccelerate = false;
            isMovingRight = true;
            curChangeDirTime = changeDirTime;
            curDeAccTime = deAccSpeedTime;
            curHSpeed = hMaxSpeed;
            bodyRoot.SetActive(true);
            deathFx.gameObject.SetActive(false);
            ChangeDirSpr();
            SetThemeColor();
        }

        public void PlayerDie()
        {
            bodyRoot.SetActive(false);
            deathFx.gameObject.SetActive(true);
            deathFx.Play();
            AudioManager.instance.PlayEffect("dead");
        }

        private float finishLineOriginSpeed;
        public void PlayerFinish()
        {
            finishLineOriginSpeed = curHSpeed;
            finishFreeMoveTime = 0f;
            isMovingRight = !isMovingRight;
            ChangeDirSpr();
        }
    }
}