using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enut4LJR
{
    public class HowtoplayPanelMgr : MonoBehaviour
    {
        [SerializeField] private Button nextBtn;
        [SerializeField] private Button prevBtn;
        [SerializeField] private Image howtoImg;
        [SerializeField] private Sprite[] howtoList;

        [SerializeField] private Text howtoText;
        private string[] howtoDialog = new string[5];

        [SerializeField] private Button goBackBtn;
        [SerializeField]PanelGroupMgr panelGroup;

        private int imageIdx = 0;

        void Awake() => AwakeFunc();

        void AwakeFunc()
		{
            if (!nextBtn) nextBtn = transform.Find("NextBtn").GetComponent<Button>();
            if (!prevBtn) prevBtn = transform.Find("PrevBtn").GetComponent<Button>();
            if (!howtoImg) howtoImg = transform.Find("HowtoImg").GetComponent<Image>();
            if (!howtoText) howtoText = transform.Find("HowtoText").GetComponent<Text>();

            if (!goBackBtn) goBackBtn = transform.Find("GoBackBtn").GetComponent<Button>();
            if (!panelGroup) panelGroup = GetComponentInParent<PanelGroupMgr>();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (nextBtn != null)
                nextBtn.onClick.AddListener(() =>
                {
                    if (imageIdx >= howtoList.Length - 1) return;
                    SoundManager.instance.PlayerSound("ValueControl");
                    imageIdx++;
                    HowtoUpdate(imageIdx);
                });

            if (prevBtn != null)
                prevBtn.onClick.AddListener(() =>
                {
                    if (imageIdx <= 0) return;
                    SoundManager.instance.PlayerSound("ValueControl");
                    imageIdx--;
                    HowtoUpdate(imageIdx);
                });

            if (goBackBtn != null)
                goBackBtn.onClick.AddListener(() =>
                {
                    SoundManager.instance.PlayerSound("Button", .3f);
                    panelGroup.PanelOn(0);
                });
            InitDialogText();
            HowtoUpdate(0);
        }

        //void Update() => UpdateFunc();


        // Update is called once per frame
        void UpdateFunc()
        {
        
        }

        void HowtoUpdate(int idx)
		{
            if (idx < 0 || idx >= howtoList.Length) return;

            if (idx == 0) prevBtn.interactable = false;
            else prevBtn.interactable = true;

            if (idx == howtoList.Length - 1) nextBtn.interactable = false;
            else nextBtn.interactable = true;

            howtoImg.sprite = howtoList[idx];
            howtoText.text = howtoDialog[idx];
		}

		private void OnEnable()
		{
            imageIdx = 0;
            HowtoUpdate(imageIdx);
		}

        void InitDialogText()
		{
            howtoDialog[0] = "달고나 게임은 찌르기 버튼을 눌러\n남들보다 더 빨리 모양을 만드는 게임이에요.";
            howtoDialog[1] = "오랫동안 누르고 있으면 피버상태가 되어\n진행속도는 빨라지지만 그만큼 피로도가 빨리 누적돼요!";
            howtoDialog[2] = "피버상태에 너무 오래 있다간\n달고나가 깨져버리니 잘 조절해야 해요 ㅠ0ㅠ";
            howtoDialog[3] = "그렇다고 느긋하게 하다간\n모양을 완성하지 못할 수도 있답니다?";
            howtoDialog[4] = "집중력을 발휘해서 제한시간 안에\n주어진 뽑기를 완성해보아요~!!";
		}
	}
}
