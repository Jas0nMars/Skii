using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public TextMeshPro scoreTxt;

    public void ShowScore(Vector3 startPos , int score){
        scoreTxt.text = "+" + score;
        transform.position = startPos;
        transform.gameObject.SetActive(true);
        transform.DOMoveY(startPos.y + 0.5f, 1).SetEase(Ease.OutCubic).onComplete = ()=>{
            transform.gameObject.SetActive(false);  
            GameManager.Instance.RecycleScorePre(this);
        };
    }
}
