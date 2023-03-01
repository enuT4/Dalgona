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
            howtoDialog[0] = "�ް� ������ ��� ��ư�� ����\n���麸�� �� ���� ����� ����� �����̿���.";
            howtoDialog[1] = "�������� ������ ������ �ǹ����°� �Ǿ�\n����ӵ��� ���������� �׸�ŭ �Ƿε��� ���� �����ſ�!";
            howtoDialog[2] = "�ǹ����¿� �ʹ� ���� �ִٰ�\n�ް��� ���������� �� �����ؾ� �ؿ� ��0��";
            howtoDialog[3] = "�׷��ٰ� �����ϰ� �ϴٰ�\n����� �ϼ����� ���� ���� �ִ�ϴ�?";
            howtoDialog[4] = "���߷��� �����ؼ� ���ѽð� �ȿ�\n�־��� �̱⸦ �ϼ��غ��ƿ�~!!";
		}
	}
}
