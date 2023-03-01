using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enut4LJR
{
    public class RankingObj : MonoBehaviour
    {
        [SerializeField] private Text rankingText;
        [SerializeField] private Image rankingImg;
        Color myColor = new Color32(255, 187, 71, 255);

		void Awake() => AwakeFunc();

        void AwakeFunc()
		{
            if (!rankingText) rankingText = transform.Find("RankingText").GetComponent<Text>();
            if (!rankingImg) rankingImg = this.GetComponent<Image>();
		}

		//// Start is called before the first frame update
		//void Start()
        //{
        //    
        //}
        //
        //// Update is called once per frame
        //void Update()
        //{
        //
        //}

        internal void DisplayRanking(string nick, int rank, float process, float time)
		{
            if (GlobalData.nickName == nick)
			{
                rankingImg.color = myColor;
				rankingText.color = Color.white;
                rankingText.GetComponent<Outline>().enabled = true;
			}
			else
			{
                rankingImg.color = Color.white;
				rankingText.color = Color.black;
                rankingText.GetComponent<Outline>().enabled = false;
			}

            rankingText.text = rank + ". " + nick + " : " + process.ToString("F2") + "%, " + time.ToString("F2") + "√ \n";
            //rankingText.text = rank + ". <color=#ff0000>" + nick + "</color> : <color=#ff0000>" + process.ToString("F2") + "</color>%, <color=#ff0000>" + time.ToString("F2") + "</color>√ \n";
        }
    }
}
