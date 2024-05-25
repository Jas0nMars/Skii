using System;
using Game;
using UnityEngine;

namespace DefaultNamespace
{
    public class EnemyRecycleBorder : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.transform.CompareTag("Enemy"))
            {
                //TODO 先暴力销毁
                GameManager.Instance.RecycleEnemyPre(other.GetComponent<Enemy>());
                GameManager.Instance.GenerateEnemy();
            }
        }

    }
}