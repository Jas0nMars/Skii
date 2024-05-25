using System;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class Enemy : MonoBehaviour
    {
        public Color color;
        public float scale = 1f;
        public Vector3 punchScale = new Vector3(0.03f, 0.03f, 0.03f);
        public float dura = 0.2f;
        public float elasticity = 1f;

        public GameObject halo;

        public SpriteRenderer bodySpr;
        public SpriteRenderer faceSpr;

        public float deathDistance = 0.5f;
        private void Awake()
        {
            transform.localScale = Vector3.one * scale;
            bodySpr = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //1.自身动画 2.弹出分数 
            transform.DOPunchScale(Vector3.one * 0.05f, 0.4f, 1, 1);
            halo.SetActive(true);
            halo.GetComponent<Animation>().Play();
            GameManager.Instance.ShowScore(transform.position, Config.EnemyTouchScore);
            GameManager.Instance.ShowTips();
        }

        private void OnTriggerStay2D(Collider2D other) {
            //判断中心点
            if(Vector2.Distance(other.transform.position, transform.position) < deathDistance && GameManager.Instance.CurState == GameManager.GameState.Running)
            {
                GameManager.Instance.PlayerDie();
            }
        }

        public void SetColor(Color color , int currentOrder)
        {
            bodySpr.color = color;
            bodySpr.sortingOrder = currentOrder;
            faceSpr.sortingOrder = currentOrder + 1;
        }
    }
}