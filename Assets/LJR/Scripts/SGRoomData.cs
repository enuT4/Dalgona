using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enut4LJR
{
    public class SGRoomData : MonoBehaviour
    {
        [HideInInspector] public string roomName = "";
        [HideInInspector] public int betGold = 100;
        [HideInInspector] public int connectPlayer = 0;
        [HideInInspector] public int maxPlayer = 0;
        [HideInInspector] private Button thisBtn;
        //[SerializeField] private Text betGoldTxt;

        public Text m_DataRoomName;
        public Image m_IconImg;
        [SerializeField] private Sprite[] iconList = new Sprite[2];
        [SerializeField] private Image banImg;

        [HideInInspector] public string m_ReadyState = "";

        public Image[] m_UserStateImgs;
        public Sprite[] m_UserStateKind;

        [SerializeField] private GameObject messPanelObj;

        private void Awake() => AwakeFunc();

		private void AwakeFunc()
		{
            //if (!betGoldTxt) betGoldTxt = transform.Find("DataRoomKind").transform.Find("MoneyTxt").GetComponent<Text>();
            if (!messPanelObj) messPanelObj = GameObject.Find("Canvas").transform.Find("MessagePanel").gameObject;
            if (!thisBtn) thisBtn = this.GetComponent<Button>();
            if (!banImg) banImg = transform.Find("DataRoomKind").transform.Find("BanImg").GetComponent<Image>();
		}

		// Start is called before the first frame update
		void Start()
        {
            thisBtn.onClick.AddListener(() =>
            {
                if (GlobalData.myGold < 100)
				{
                    messPanelObj.SetActive(true);
                    return;
				}
                SGPhotonInit RefPtInit = FindObjectOfType<SGPhotonInit>();
                if (RefPtInit != null)
                    RefPtInit.OnClickRoomItem(roomName);
            });
        }

        //// Update is called once per frame
        //void Update()
        //{
        //
        //}


        public void DispRoomData(bool a_isOpen)
		{
            if (a_isOpen)
			{
				m_DataRoomName.color = new Color32(0, 0, 0, 255);
                if (!thisBtn.IsInteractable())
                    thisBtn.interactable = true;
			}
			else
			{
                m_DataRoomName.color = new Color32(0, 0, 255, 255);
                if (thisBtn.IsInteractable())
                    thisBtn.interactable = false;
            }

            m_DataRoomName.text = roomName;
            //betGoldTxt.text = betGold.ToString();


            if (10 < maxPlayer) return;

            for (int ii = 0; ii < 10; ii++)
			{
                if (ii < connectPlayer)
                    m_UserStateImgs[ii].sprite = m_UserStateKind[0];
                else if (ii < maxPlayer)
                    m_UserStateImgs[ii].sprite = m_UserStateKind[1];
                else
                    m_UserStateImgs[ii].sprite = m_UserStateKind[2];
			}
            if (connectPlayer >= maxPlayer)
                thisBtn.interactable = false;

            if (!thisBtn.IsInteractable() || !a_isOpen)
			{
                m_IconImg.sprite = iconList[1];
                banImg.gameObject.SetActive(true);
			}
            else
			{
                m_IconImg.sprite = iconList[0];
                banImg.gameObject.SetActive(false);
			}

        }
    }
}
