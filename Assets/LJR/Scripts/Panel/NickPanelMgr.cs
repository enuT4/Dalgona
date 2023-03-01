using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enut4LJR
{
    public class NickPanelMgr : MonoBehaviour
    {
        [SerializeField] private InputField nickInputField;
        [SerializeField] private Button startBtn;

        [SerializeField] private Button goBackBtn;
        PanelGroupMgr panelGroup;

        [SerializeField] private Text messageText;
        string message;
        WaitForSeconds messageTxtWFS = new WaitForSeconds(3.0f);

        GameObject fadePanelObj;
        FadePanel fadePnl;

        void Awake() => AwakeFunc();

        void AwakeFunc()
		{
            if (!nickInputField) nickInputField = transform.Find("NicknameIF").GetComponent<InputField>();
            if (!startBtn) startBtn = transform.Find("StartBtn").GetComponent<Button>();

            if (!goBackBtn) goBackBtn = transform.Find("GoBackBtn").GetComponent<Button>();
            if (!panelGroup) panelGroup = GetComponentInParent<PanelGroupMgr>();

            if (!messageText) messageText = transform.Find("MessageText").GetComponent<Text>();

            if (!fadePanelObj) fadePanelObj = GameObject.Find("Canvas").transform.Find("FadeOutPanel").gameObject;
            if (!fadePnl) fadePnl = fadePanelObj.GetComponent<FadePanel>();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (messageText != null) messageText.gameObject.SetActive(false);
            if (startBtn != null)
                startBtn.onClick.AddListener(GameStartFunc);
            if (goBackBtn != null)
                goBackBtn.onClick.AddListener(() =>
                {
                    SoundManager.instance.PlayerSound("Button", .3f);
                    panelGroup.PanelOn(0);
                });
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void GameStartFunc()
        {
            SoundManager.instance.PlayerSound("Button", 0.3f);
            if (messageText.gameObject.activeSelf) return;
            if (string.IsNullOrEmpty(nickInputField.text))
            {
                message = "닉네임을 입력해주세요.";
                StartCoroutine(ShowMessageCo());
            }
            else
            {
                GlobalData.nickName = nickInputField.text;
                fadePanelObj.SetActive(true);
                fadePnl.StartFade(1, "SGLobbyScene");
            }


        }

        IEnumerator ShowMessageCo()
        {
            messageText.text = message;
            messageText.gameObject.SetActive(true);
            yield return messageTxtWFS;
            messageText.gameObject.SetActive(false);
            message = "";

            yield return null;
        }
    }
}
