using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enut4LJR
{
    public class PanelGroupMgr : MonoBehaviour
    {
        //Panel
        [SerializeField] private MainPanelMgr mainPanel;
        [SerializeField] private HowtoplayPanelMgr howtoplayPanel;
        [SerializeField] private SettingPanelMgr settingPanel;
        [SerializeField] private CreditPanelMgr creditPanel;
        [SerializeField] private GameObject nickPanel;
        GameObject[] panelList = new GameObject[5];

        private void Awake() => AwakeFunc();

        private void AwakeFunc()
        {
            if (!mainPanel) mainPanel = transform.Find("MainPanel").GetComponent<MainPanelMgr>();
            if (!howtoplayPanel) howtoplayPanel = transform.Find("HowtoplayPanel").GetComponent<HowtoplayPanelMgr>();
            if (!settingPanel) settingPanel = transform.Find("SettingPanel").GetComponent<SettingPanelMgr>();
            if (!creditPanel) creditPanel = transform.Find("CreditPanel").GetComponent<CreditPanelMgr>();
            if (!nickPanel) nickPanel = transform.Find("NickPanel").gameObject;
            InitPanelList();
        }

        // Start is called before the first frame update
        void Start()
        {
            PanelOn(0);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void InitPanelList()
        {
            panelList[0] = mainPanel.gameObject;
            panelList[1] = nickPanel.gameObject; 
            panelList[2] = howtoplayPanel.gameObject; 
            panelList[3] = settingPanel.gameObject; 
            panelList[4] = creditPanel.gameObject;
        }

        internal void PanelOn(int panelIdx)
        {
            if (panelIdx < 0 || panelIdx >= panelList.Length) return;

            for (int ii = 0; ii < panelList.Length; ii++)
                panelList[ii].SetActive(false);

            panelList[panelIdx].SetActive(true);
        }
    }
}
