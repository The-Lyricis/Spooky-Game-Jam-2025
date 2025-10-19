using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EndUi : MonoBehaviour
{
    [SerializeField] private GameObject EndPanel;
    [SerializeField] private Text text1;
    int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClick()
    {
        if(index==0)
        {
            text1.gameObject.GetComponent<Text>().DOFade(0, 0.2f).OnComplete(() =>
                    {
                        text1.text = "Thank you for playing.";
                        text1.gameObject.GetComponent<Text>().DOFade(1, 0.2f);
                    });
        index++;
        }
        else if(index==1)
        {
            text1.gameObject.GetComponent<Text>().DOFade(0, 0.2f).OnComplete(() =>
                    {
                        text1.text = "Game Credits:\nGu Zhengyang,\nGuo Yuxi,\nWan Hang,\nWang Ao,\nYe Jiliang";
                        text1.gameObject.GetComponent<Text>().DOFade(1, 0.2f);
                    });
            index++;
            
        }
        else if(index==2)
        {
            text1.gameObject.GetComponent<Text>().DOFade(0, 0.2f).OnComplete(() =>
                    {
                        text1.text = "Click to quit";
                        text1.gameObject.GetComponent<Text>().DOFade(1, 0.2f);
                    });
            index++;
        }
        else if(index==3)
        {
            Debug.Log("[MainMenu] Quitting game");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
