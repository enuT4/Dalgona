using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enut4LJR
{
    public class SGUserData : MonoBehaviour
    {
        //유저 정보
        [HideInInspector] public string nickName = "";
        [HideInInspector] public int m_UniqueID = -1;
        [HideInInspector] public bool m_IamReady = false;

        public Text m_UserNickTxt;
        //public Text m_UserGoldTxt;
        public GameObject m_isOverMaxObj;
        public Image m_CrossImg;
        public Text m_UserReadyTxt;

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

        public void DispUserData(bool a_isMine, string a_UserNick)
		{
            if (a_isMine) m_UserNickTxt.color = Color.green;

            m_UserNickTxt.text = a_UserNick;
            string a_GoldTxt = "";
            //if (a_Gold >= 100000)
            //    a_GoldTxt = (a_Gold / 1000).ToString("N1") + " K";
            //else
            //    a_GoldTxt = a_Gold.ToString("N0");
            //m_UserGoldTxt.text = a_GoldTxt;
            
            //if (a_isOverMax)
			//{
            //    m_isOverMaxObj.SetActive(!a_isOverMax);
            //    m_CrossImg.gameObject.SetActive(a_isOverMax);
            //}

            m_UserReadyTxt.gameObject.SetActive(m_IamReady);
        }
    }
}
