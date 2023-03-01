using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enut4LJR
{
    public class CreateRoom : MonoBehaviour
    {
        [SerializeField] private Text rmNamePH;
        [SerializeField] private InputField rmNameIF;

        //[SerializeField] private Toggle pwToggle;
        //[SerializeField] private Text pwLabel;
        //[SerializeField] private InputField pwIF;
        //[SerializeField] private Text pwPH;

        [SerializeField] private Button maxPlayUpBtn;
        [SerializeField] private Button maxPlayDownBtn;
        [SerializeField] private InputField maxPlayIF;
        [SerializeField] private Text maxPlayPH;

        //[SerializeField] private Button goldUpBtn;
        //[SerializeField] private Button goldDownBtn;
        //[SerializeField] private InputField goldIF;
        //[SerializeField] private Text goldPH;

        [SerializeField] private Button closeBtn;
        [SerializeField] private Button createBtn;
        private Transform crImgTr;

        [SerializeField] private GameObject messBox;
        [SerializeField] private Button messOKBtn;

        int betGold = 100;
        string rmName = "";
        //string pwEx = "";
        int maxPlayers = 8;

        //방 정보
        string tempRoomName = "";
        //string tempPassword = "";
        int tempBetGold = 0;
        int tempMaxPlayer = 0;

        SGPhotonInit photonInit;

        void Awake() => AwakeFunc();

        void AwakeFunc()
		{
            if (!crImgTr) crImgTr = transform.Find("CreateImg").transform;
            if (!rmNameIF) rmNameIF = crImgTr.Find("RmNameIF").GetComponent<InputField>();
            if (!rmNamePH) rmNamePH = rmNameIF.transform.Find("Placeholder").GetComponent<Text>();

            //if (!pwToggle) pwToggle = crImgTr.Find("Toggle").GetComponent<Toggle>();
            //if (!pwLabel) pwLabel = crImgTr.Find("RmPWLabel").GetComponent<Text>();
            //if (!pwIF) pwIF = crImgTr.Find("RmPWIF").GetComponent<InputField>();
            //if (!pwPH) pwPH = pwIF.transform.Find("Placeholder").GetComponent<Text>();

            if (!maxPlayUpBtn) maxPlayUpBtn = crImgTr.Find("PlayerUpBtn").GetComponent<Button>();
            if (!maxPlayDownBtn) maxPlayDownBtn = crImgTr.Find("PlayerDownBtn").GetComponent<Button>();
            if (!maxPlayIF) maxPlayIF = crImgTr.Find("MaxPlayerIF").GetComponent<InputField>();
            if (!maxPlayPH) maxPlayPH = maxPlayIF.transform.Find("Placeholder").GetComponent<Text>();

            //if (!goldUpBtn) goldUpBtn = crImgTr.Find("BetGoldUpBtn").GetComponent<Button>();
            //if (!goldDownBtn) goldDownBtn = crImgTr.Find("BetGoldDownBtn").GetComponent<Button>();
            //if (!goldIF) goldIF = crImgTr.Find("BetGoldIF").GetComponent<InputField>();
            //if (!goldPH) goldPH = goldIF.transform.Find("Placeholder").GetComponent<Text>();

            if (!closeBtn) closeBtn = crImgTr.Find("CloseBtn").GetComponent<Button>();
            if (!createBtn) createBtn = crImgTr.Find("CreateBtn").GetComponent<Button>();

            if (!messBox) messBox = transform.Find("MessageBox").gameObject;
            if (!messOKBtn) messOKBtn = messBox.transform.Find("Button").GetComponent<Button>();

            if (!photonInit) photonInit = FindObjectOfType<SGPhotonInit>();
        }


        // Start is called before the first frame update
        void Start()
        {
            //if (goldUpBtn != null)
            //    goldUpBtn.onClick.AddListener(() => AdjGoldFunc(true));
            //if (goldDownBtn != null)
            //    goldDownBtn.onClick.AddListener(() => AdjGoldFunc(false));

            if (maxPlayUpBtn != null)
                maxPlayUpBtn.onClick.AddListener(() => AdjMaxFunc(true));
            if (maxPlayDownBtn != null)
                maxPlayDownBtn.onClick.AddListener(() => AdjMaxFunc(false));

            if (closeBtn != null)
                closeBtn.onClick.AddListener(() => gameObject.SetActive(false));
            if (createBtn != null)
                createBtn.onClick.AddListener(CreateFunc);

            //if (goldPH != null)
            //    goldPH.text = betGold.ToString();

            rmName = "Room_" + Random.Range(0, 999).ToString("000");
            //pwEx = "Enter Password";
            if (rmNamePH != null)
                rmNamePH.text = rmName;

            //if (pwToggle != null)
            //    pwToggle.onValueChanged.AddListener(ToggleFunc);
            //pwToggle.isOn = false;

            if (messBox != null)
                messBox.SetActive(false);

            if (messOKBtn != null)
                messOKBtn.onClick.AddListener(() => messBox.SetActive(false));
        }

        // Update is called once per frame
        void Update()
        {
            //if (goldIF.isFocused) goldPH.text = "";
            //else
			//{
            //    if (!string.IsNullOrEmpty(goldIF.text))
            //        betGold = int.Parse(goldIF.text);
            //    if (goldPH.text != betGold.ToString())
			//	{
            //        goldIF.text = "";
            //        if (betGold > GlobalData.myGold / 2) betGold = GlobalData.myGold / 2;
            //        goldPH.text = betGold.ToString();
			//	}
			//}

            if (rmNameIF.isFocused) rmNamePH.text = "";
            else
            {
                if (string.IsNullOrEmpty(rmNameIF.text))
                    rmNamePH.text = rmName;
            }

            if (maxPlayIF.isFocused) maxPlayPH.text = "";
            else
            {
                if (!string.IsNullOrEmpty(maxPlayIF.text))
                    maxPlayers = int.Parse(maxPlayIF.text);
                if (maxPlayPH.text != maxPlayers.ToString())
                {
                    maxPlayIF.text = "";
                    if (maxPlayers > 8) maxPlayers = 8;
                    if (maxPlayers < 2) maxPlayers = 2;
                    maxPlayPH.text = maxPlayers.ToString();
                }
            }

            //if (pwIF.isFocused) pwPH.text = "";
            //else
            //{
            //    if (string.IsNullOrEmpty(pwIF.text))
            //        pwPH.text = pwEx;
            //}


        }

        //void AdjGoldFunc(bool isUp)
		//{
        //    if (isUp)
		//	{
        //        betGold += 100;
        //        if (betGold > GlobalData.myGold / 2) betGold = GlobalData.myGold / 2;
		//	}
        //    else
		//	{
        //        betGold -= 100;
        //        if (betGold < 100) betGold = 100;
		//	}
        //}

        void AdjMaxFunc(bool isUp)
		{
            if (isUp)
			{
                maxPlayers++;
                if (maxPlayers > 8) maxPlayers = 8;
			}
            else
			{
                maxPlayers--;
                if (maxPlayers < 2) maxPlayers = 2;
			}
		}

        void CreateFunc()
		{
            //if (pwToggle.isOn)
			//{
            //    if (string.IsNullOrEmpty(pwIF.text))
            //    {
            //        messBox.SetActive(true);
            //        return;
            //    }
            //}
            if (!string.IsNullOrEmpty(rmNameIF.text))
                tempRoomName = rmNameIF.text;
            else
                tempRoomName = rmName;
            //tempPassword = pwIF.text;
            tempBetGold = betGold;
            tempMaxPlayer = maxPlayers;
            if (photonInit != null)
                photonInit.CreateRoomFunc(tempRoomName, tempMaxPlayer, tempBetGold);
            //if (photonInit != null)
            //    photonInit.CreateRoomFunc(tempRoomName, tempPassword, tempBetGold);
            //ADebug.Log("방생성 : " + tempRoomName + ", 비번 : " + tempPassword + ", 판돈 : " + tempBetGold);
		}

        //void ToggleFunc(bool isOn)
		//{
        //    if (isOn)
		//	{
        //        pwLabel.text = "Password";
        //        pwIF.interactable = true;
		//	}
        //    else
		//	{
        //        pwLabel.text = "Password?";
        //        pwIF.interactable = false;
        //    }
		//}
    }
}
