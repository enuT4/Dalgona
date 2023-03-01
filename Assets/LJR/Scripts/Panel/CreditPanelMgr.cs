using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enut4LJR
{
    public class CreditPanelMgr : MonoBehaviour
    {
        [SerializeField] private Button goBackBtn;
        PanelGroupMgr panelGroup;

		void Awake() => AwakeFunc();

        void AwakeFunc()
		{
            if (!goBackBtn) goBackBtn = transform.Find("GoBackBtn").GetComponent<Button>();
            if (!panelGroup) panelGroup = GetComponentInParent<PanelGroupMgr>();
        }

		// Start is called before the first frame update
		void Start()
        {
            if (goBackBtn != null)
                goBackBtn.onClick.AddListener(() =>
                {
                    SoundManager.instance.PlayerSound("Button", .3f);
                    panelGroup.PanelOn(0);
                });
        }

        //void Update() => UpdateFunc();

        // Update is called once per frame
        void UpdateFunc()
        {
        
        }
    }
}
