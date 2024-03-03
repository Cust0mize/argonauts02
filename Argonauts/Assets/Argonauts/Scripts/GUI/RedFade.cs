using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedFade : MonoBehaviour {
    [SerializeField]
    private int countPing;
    [SerializeField]
    private float speedFade=3;
    private IEnumerator ping;

    public void StartPing()
    {
        if(ping!=null)
        {
            StopCoroutine(ping);
            ping = null;
        }
        ping = Ping();
        StartCoroutine(Ping());
    }

    private IEnumerator Ping()
    {
        int plays = 0;
        bool increase = true;
        Color col = GetComponent<Image>().color;

        while (plays < countPing)
        {
            if (increase)
            {
                col.a += speedFade * Time.deltaTime;
                if (col.a >= 1F)
                {
                    increase = false;
                    plays++;
                }
            }
            else
            {
                col.a -= speedFade * Time.deltaTime;
                if (col.a <= 0)
                {
                    increase = true;
                    plays++;
                }
            }
            GetComponent<Image>().color = col;
            yield return null;
        }
        GetComponent<Image>().color = new Color (1,1,1,0);
    }
}
