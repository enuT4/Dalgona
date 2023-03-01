using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enut4LJR
{
    public class MainPanelMgr : MonoBehaviour
    {
        [SerializeField] private Button gamestartBtn;
        [SerializeField] private Button howtoplayBtn;
        [SerializeField] private Button settingBtn;
        [SerializeField] private Button creditBtn;

        PanelGroupMgr panelGroup;

        void Awake() => AwakeFunc();

        void AwakeFunc()
		{
            if (!gamestartBtn) gamestartBtn = transform.Find("GameStartBtn").GetComponent<Button>();
            if (!howtoplayBtn) howtoplayBtn = transform.Find("HowtoPlayBtn").GetComponent<Button>();
            if (!settingBtn) settingBtn = transform.Find("SettingBtn").GetComponent<Button>();
            if (!creditBtn) creditBtn = transform.Find("CreditBtn").GetComponent<Button>();
            if (!panelGroup) panelGroup = GetComponentInParent<PanelGroupMgr>();
		}

        // Start is called before the first frame update
        void Start()
        {
            if (gamestartBtn != null)
                gamestartBtn.onClick.AddListener(() =>
                {
                    SoundManager.instance.PlayerSound("Button", .3f);
                    panelGroup.PanelOn(1);
                });
            if (howtoplayBtn != null)
                howtoplayBtn.onClick.AddListener(() =>
                {
                    SoundManager.instance.PlayerSound("Button", .3f);
                    panelGroup.PanelOn(2);
                });
            if (settingBtn != null)
                settingBtn.onClick.AddListener(() =>
                {
                    SoundManager.instance.PlayerSound("Button", .3f);
                    panelGroup.PanelOn(3);
                });
            if (creditBtn != null)
                creditBtn.onClick.AddListener(() =>
                {
                    SoundManager.instance.PlayerSound("Button", .3f);
                    panelGroup.PanelOn(4);
                });
        }

        //void Update() => UpdateFunc();

        // Update is called once per frame
        void UpdateFunc()
        {
        
        }
    }
}
