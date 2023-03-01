using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enut4LJR
{
    public class SettingPanelMgr : MonoBehaviour
    {
        [SerializeField] private Button goBackBtn;
        PanelGroupMgr panelGroup;

        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider effectSlider;
        [SerializeField] private Button masterMuteBtn;
        [SerializeField] private Button musicMuteBtn;
        [SerializeField] private Button effectMuteBtn;
        Image masterMuteImg;
        Image musicMuteImg;
        Image effectMuteImg;
        bool isMasterMute = false;
        bool isMusicMute = false;
        bool isEffectMute = false;

        [SerializeField] private Button applyBtn;
        private AudioSource audioSource;

        //MessageBox
        [SerializeField] private GameObject messageBoxObj;
        [SerializeField] private Button messYesBtn;
        [SerializeField] private Button messNoBtn;

        [SerializeField] private Text messageText;
        WaitForSeconds messageTxtWFS = new WaitForSeconds(3.0f);


        [SerializeField] private Sprite[] muteList = new Sprite[2];

        private void Awake() => AwakeFunc();

        void AwakeFunc()
		{
            if (!goBackBtn) goBackBtn = transform.Find("GoBackBtn").GetComponent<Button>();
            if (!panelGroup) panelGroup = GetComponentInParent<PanelGroupMgr>();
            if (!masterSlider) masterSlider = transform.Find("MasterSlider").GetComponent<Slider>();
            if (!musicSlider) musicSlider = transform.Find("MusicSlider").GetComponent<Slider>();
            if (!effectSlider) effectSlider = transform.Find("EffectSlider").GetComponent<Slider>();
            if (!masterMuteBtn) masterMuteBtn = transform.Find("MasterMuteBtn").GetComponent<Button>();
            if (!musicMuteBtn) musicMuteBtn = transform.Find("MusicMuteBtn").GetComponent<Button>();
            if (!effectMuteBtn) effectMuteBtn = transform.Find("EffectMuteBtn").GetComponent<Button>();
            if (!masterMuteImg) masterMuteImg = masterMuteBtn.GetComponent<Image>();
            if (!musicMuteImg) musicMuteImg = musicMuteBtn.GetComponent<Image>();
            if (!effectMuteImg) effectMuteImg = effectMuteBtn.GetComponent<Image>();
            if (!applyBtn) applyBtn = transform.Find("ApplyBtn").GetComponent<Button>();

            if (!messageBoxObj) messageBoxObj = transform.Find("MessageBox").gameObject;
            if (!messYesBtn) messYesBtn = messageBoxObj.transform.Find("YesBtn").GetComponent<Button>();
            if (!messNoBtn) messNoBtn = messageBoxObj.transform.Find("NoBtn").GetComponent<Button>();

            if (!messageText) messageText = transform.Find("MessageText").GetComponent<Text>();

            if (!audioSource && MusicManager.instance)
                if (MusicManager.instance.gameObject.TryGetComponent(out audioSource))
                    return;
        }

		// Start is called before the first frame update
		void Start()
        {
            if (goBackBtn != null)
                goBackBtn.onClick.AddListener(() =>
                {
                    SoundManager.instance.PlayerSound("Button", .3f);
                    if (IsSettingDiff()) messageBoxObj.SetActive(true);
                    else panelGroup.PanelOn(0);
                });

            if (masterMuteBtn != null)
                masterMuteBtn.onClick.AddListener(() => MuteFunc(1));
            if (musicMuteBtn != null)
                musicMuteBtn.onClick.AddListener(() => MuteFunc(2));
            if (effectMuteBtn != null)
                effectMuteBtn.onClick.AddListener(() => MuteFunc(3));

            if (messYesBtn != null)
                messYesBtn.onClick.AddListener(() =>
                {
                    messageBoxObj.gameObject.SetActive(false);
                    ApplyBtnFunc();
                    panelGroup.PanelOn(0);
                });
            if (messNoBtn != null)
                messNoBtn.onClick.AddListener(() => messageBoxObj.gameObject.SetActive(false));

            if (applyBtn != null)
                applyBtn.onClick.AddListener(ApplyBtnFunc);

            InitSettingFunc();
        }

        // Update is called once per frame
        void Update()
        {
            if (isMasterMute || isMusicMute)
                audioSource.volume = 0.0f;
            else
                audioSource.volume = .3f * (masterSlider.value * musicSlider.value);
        }

        void InitSettingFunc()
		{
            masterSlider.value = GlobalData.masterVolume;
            musicSlider.value = GlobalData.musicVolume;
            effectSlider.value = GlobalData.effectVolume;
            isMasterMute = GlobalData.masterMute;
            isMusicMute = GlobalData.musicMute;
            isEffectMute = GlobalData.effectMute;
            SetButtonSprite(masterMuteImg, isMasterMute);
            SetButtonSprite(musicMuteImg, isMusicMute);
            SetButtonSprite(effectMuteImg, isEffectMute);

            if (messageBoxObj.activeSelf) messageBoxObj.SetActive(false);
            if (messageText.gameObject.activeSelf) messageText.gameObject.SetActive(false);
        }

        void MuteFunc(int buttonIdx)
		{
            if (buttonIdx == 1)
            {
                isMasterMute = !isMasterMute;
                SetButtonSprite(masterMuteImg, isMasterMute);
            }
            else if (buttonIdx == 2)
            {
                isMusicMute = !isMusicMute;
                SetButtonSprite(musicMuteImg, isMusicMute);
            }
            else if (buttonIdx == 3)
            {
                isEffectMute = !isEffectMute;
                SetButtonSprite(effectMuteImg, isEffectMute);
            }
            else return;
		}

        void SetButtonSprite(Image img, bool isMute)
		{
            if (isMute) img.sprite = muteList[1];
            else img.sprite = muteList[0];
		}

        void ApplyBtnFunc()
		{
            if (IsSettingDiff())
			{
                GlobalData.masterVolume = masterSlider.value;
                GlobalData.musicVolume = musicSlider.value;
                GlobalData.effectVolume = effectSlider.value;
                GlobalData.masterMute = isMasterMute;
                GlobalData.musicMute = isMusicMute;
                GlobalData.effectMute = isEffectMute;
                StopAllCoroutines();
                StartCoroutine(ShowMessageCo());
            }
            SoundManager.instance.PlayerSound("Button", .3f);
        }

		private void OnEnable()
		{
            InitSettingFunc();
		}

        bool IsSettingDiff()
		{
            if (GlobalData.masterMute != isMasterMute)
                return true;
            if (GlobalData.musicMute != isMusicMute)
                return true;
            if (GlobalData.effectMute != isEffectMute)
                return true;

            if (GlobalData.masterVolume != masterSlider.value)
                return true;
            if (GlobalData.musicVolume != musicSlider.value)
                return true;
            if (GlobalData.effectVolume != effectSlider.value)
                return true;


            return false;
		}

        IEnumerator ShowMessageCo()
        {
            messageText.gameObject.SetActive(true);
            yield return messageTxtWFS;
            messageText.gameObject.SetActive(false);

            yield return null;
        }

    }
}
