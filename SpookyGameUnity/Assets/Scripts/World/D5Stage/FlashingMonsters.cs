using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class FlashingMonsters : MonoBehaviour
{
    [SerializeField] private GameObject Monster1;
    [SerializeField] private GameObject Monster2;

    [SerializeField] private GameObject bigbigMonster;
    [SerializeField] private GameObject bigbigMonster2;
    
    [SerializeField] private GameObject EyesPanel;
    void Start()
    {
        bigbigMonster.SetActive(true);
        bigbigMonster2.SetActive(false);
        StartCoroutine(FlashMonsters());
        
    }
    IEnumerator FlashMonsters()
    {
        
        Monster1.GetComponent<SpriteRenderer>().DOFade(0, 0.2f).OnComplete(() =>
        {
            Monster1.GetComponent<SpriteRenderer>().DOFade(1, 0.2f);
        });
        Monster2.GetComponent<SpriteRenderer>().DOFade(0, 0.2f).OnComplete(() =>
        {
            Monster2.GetComponent<SpriteRenderer>().DOFade(1, 0.2f);
        }); 

        yield return new WaitForSeconds(1f);
        Monster1.GetComponent<SpriteRenderer>().DOFade(0, 0.2f).OnComplete(() =>
        {
            Monster1.GetComponent<SpriteRenderer>().DOFade(1, 0.2f);
        });

        Monster2.GetComponent<SpriteRenderer>().DOFade(0, 0.2f).OnComplete(() =>
        {
            Monster2.GetComponent<SpriteRenderer>().DOFade(1, 0.2f);
        }); 
        yield return new WaitForSeconds(1f);

        
        Monster1.GetComponent<SpriteRenderer>().DOFade(1, 0.2f);
        Monster2.GetComponent<SpriteRenderer>().DOFade(1, 0.2f);
        yield return new WaitForSeconds(1f);

        EyesPanel.SetActive(true);
        
        
    }
}
