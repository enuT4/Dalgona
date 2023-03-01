using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Enut4LJR
{
    public class SGPhotonInit : MonoBehaviourPunCallbacks
    {
        public static bool isFocus = true;

        //public InputField m_UserIDIF;
        public Button m_JoinRandRoomBtn;

        //public InputField m_roomNameIF;
        public Button m_CreateRoomBtn;

        public Button m_SearchRoomBtn;

        public GameObject m_ScrollContents;

        public GameObject m_RoomObj;
        List<RoomInfo> myRoomList = new List<RoomInfo>();

        [SerializeField] GameObject fadeObj;
        FadePanel panel;

        [SerializeField] private Text nickNameTxt;
        [SerializeField] private Text myGoldTxt;

        [SerializeField] private GameObject createRoomObj;

        [SerializeField] private GameObject messagePanel;
        [SerializeField] private Button messageOKBtn;

        int tempGold = 0;


		void Awake()
		{
			if (!PhotonNetwork.IsConnected)
                PhotonNetwork.ConnectUsingSettings();

            //m_UserIDIF.text = GetUserID();

            //m_roomNameIF.text = "Room_" + Random.Range(0, 999).ToString("000");

            if (!fadeObj) fadeObj = GameObject.Find("Canvas").transform.Find("FadeOutPanel").gameObject;
            if (!panel) panel = fadeObj.GetComponent<FadePanel>();

            if (!nickNameTxt) nickNameTxt = GameObject.Find("MyInfoImage").transform.Find("NicknameBG").GetComponentInChildren<Text>();
            if (!myGoldTxt) myGoldTxt = GameObject.Find("MyInfoImage").transform.Find("MoneyBG").GetComponentInChildren<Text>();

            if (!createRoomObj) createRoomObj = GameObject.Find("Canvas").transform.Find("CreateRoomObj").gameObject;

            if (!messagePanel) messagePanel = GameObject.Find("Canvas").transform.Find("MessagePanel").gameObject;
            if (!messageOKBtn) messageOKBtn = messagePanel.transform.Find("Button").GetComponent<Button>();
        }

		// Start is called before the first frame update
		void Start()
        {
            if (m_JoinRandRoomBtn != null)
                m_JoinRandRoomBtn.onClick.AddListener(JoinRandRoomFunc);

            if (m_CreateRoomBtn != null)
                m_CreateRoomBtn.onClick.AddListener(()=>createRoomObj.SetActive(true));

            if (m_SearchRoomBtn != null)
                m_SearchRoomBtn.onClick.AddListener(SearchRoomFunc);

            if (fadeObj != null)
			{
                fadeObj.GetComponent<FadePanel>().StartFade(0);
                fadeObj.SetActive(true);
            }

            nickNameTxt.text = GlobalData.nickName;
            myGoldTxt.text = GlobalData.myGold.ToString();

            if (messageOKBtn != null)
                messageOKBtn.onClick.AddListener(() => messagePanel.SetActive(false));

            //미구현
            m_SearchRoomBtn.interactable = false;
            m_CreateRoomBtn.interactable = false;
            m_JoinRandRoomBtn.interactable = false;

            if (MusicManager.instance.audioSource.clip == null)
                MusicManager.instance.PlayMusic("BGM1");
        }

		// Update is called once per frame
		void Update()
		{
		    if (Input.GetKeyDown(KeyCode.G))
			{
                tempGold = GlobalData.myGold;
                GlobalData.myGold = 0;
			}

            if (Input.GetKeyDown(KeyCode.H))
			{
                GlobalData.myGold = tempGold;
			}
		}

		public void OnApplicationFocus(bool focus)
		{
            SGPhotonInit.isFocus = focus;
		}

		public override void OnConnectedToMaster()
		{
            ADebug.Log("서버 접속 완료");
            PhotonNetwork.JoinLobby();
		}

		public override void OnJoinedLobby()
		{
			ADebug.Log("로비 접속 완료");
            //m_UserIDIF.text = GetUserID();
            m_CreateRoomBtn.interactable = true;
            m_JoinRandRoomBtn.interactable = true;

        }

		public override void OnJoinRoomFailed(short returnCode, string message)
		{
			ADebug.Log("랜덤 방 참가 실패 (참가할 방이 존재하지 않습니다.");

            RoomOptions m_roomOptions = new RoomOptions();
            m_roomOptions.IsVisible = true;
            m_roomOptions.MaxPlayers = 8;  //방 최대 인원
            m_roomOptions.PublishUserId = true;

            PhotonNetwork.CreateRoom("MyRoom", m_roomOptions);
		}

		public override void OnJoinedRoom()
		{
			ADebug.Log("방 참가 완료");
            GlobalData.userID = PhotonNetwork.LocalPlayer.UserId;
            StartCoroutine(this.LoadSugarGameCo());
		}

        string GetUserID()
        {
            //string userID = PlayerPrefs.GetString("USER_ID");
            string userID = GlobalData.nickName;

            if (string.IsNullOrEmpty(userID))
                userID = "USER_" + Random.Range(0, 999).ToString("000");

            return userID;
        }

        public void JoinRandRoomFunc()
        {
            if (GlobalData.myGold < 100)
			{
                messagePanel.SetActive(true);
                return;
			}
            PhotonNetwork.LocalPlayer.NickName = GlobalData.nickName;
            GlobalData.userID = PhotonNetwork.LocalPlayer.UserId;
            //PlayerPrefs.SetString("USER_ID", GlobalData.nickName);
            PhotonNetwork.JoinRandomRoom();
        }


        IEnumerator LoadSugarGameCo()
		{
            PhotonNetwork.IsMessageQueueRunning = false;

            Time.timeScale = 1.0f;

            //AsyncOperation ao = SceneManager.LoadSceneAsync("SGReadyScene");
            fadeObj.SetActive(true);
            panel.StartFade(1, "SGReadyScene");
            yield return null;
		}

		void OnGUI()
		{
            string a_Str = PhotonNetwork.NetworkClientState.ToString();

			GUI.Label(new Rect(10, 1, 1500, 60),
				"<color=00ff00><size=35>" + a_Str + "</size></color>");

		}

        void SearchRoomFunc()
		{

		}

        public void CreateRoomFunc(string roomName, int maxPlayer, int betGold)
		{
            if (GlobalData.myGold < 100)
            {
                messagePanel.SetActive(true);
                return;
            }

            //string _roomName = m_roomNameIF.text;
            //if (string.IsNullOrEmpty(m_roomNameIF.text))
            //    _roomName = "Room_" + Random.Range(0, 999).ToString("000");

            PhotonNetwork.LocalPlayer.NickName = GlobalData.nickName;
            //PlayerPrefs.SetString("USER_ID", m_UserIDIF.text);
            //베팅금액 소모
            //GlobalData.myGold -= betGold;
            //myGoldTxt.text = GlobalData.myGold.ToString();

            RoomOptions m_RoomOptions = new RoomOptions();
            m_RoomOptions.IsOpen = true;
            m_RoomOptions.IsVisible = true;
            m_RoomOptions.MaxPlayers = (byte)maxPlayer;
            m_RoomOptions.PublishUserId = true;

            

            PhotonNetwork.CreateRoom(roomName, m_RoomOptions, TypedLobby.Default);
        }

		public override void OnCreateRoomFailed(short returnCode, string message)
		{
			ADebug.Log("방 만들기 실패");
			ADebug.Log(returnCode.ToString());
            ADebug.Log(message);
		}

		public override void OnRoomListUpdate(List<RoomInfo> roomList)
		{
            int roomCount = roomList.Count;

            for (int ii = 0; ii < roomCount; ii++)
			{
                if (!roomList[ii].RemovedFromList)
                {
                    if (!myRoomList.Contains(roomList[ii]))
                    {
                        GameObject a_RmObj = CreateRoomItem(roomList[ii]);
                        roomList[ii].m_RefRoomItem = a_RmObj;
                        myRoomList.Add(roomList[ii]);
                    }
                    else
                    {
                        int a_Idx = myRoomList.IndexOf(roomList[ii]);
                        GameObject a_OldRmObj = myRoomList[a_Idx].m_RefRoomItem;

                        myRoomList[a_Idx] = roomList[ii];

                        myRoomList[a_Idx].m_RefRoomItem = a_OldRmObj;
                        if (a_OldRmObj == null)
                        {
                            GameObject a_RmObj = CreateRoomItem(roomList[ii]);
                            myRoomList[a_Idx].m_RefRoomItem = a_RmObj;
                        }
                        else
                        {
                            SGRoomData roomData = a_OldRmObj.GetComponent<SGRoomData>();
                            roomData.roomName = roomList[ii].Name;
                            roomData.connectPlayer = roomList[ii].PlayerCount;
                            roomData.maxPlayer = roomList[ii].MaxPlayers;
                            

                            roomData.DispRoomData(roomList[ii].IsOpen);
                        }
                    }
                }
                else if (myRoomList.IndexOf(roomList[ii]) != -1)
                {
                    int a_Idx = myRoomList.IndexOf(roomList[ii]);
                    if (myRoomList[a_Idx].m_RefRoomItem != null)
                        Destroy(myRoomList[a_Idx].m_RefRoomItem);
                    myRoomList.RemoveAt(myRoomList.IndexOf(roomList[ii]));
				}

			}
		}

        public void OnClickRoomItem(string roomName)
        {
            //로컬 플레이어의 이름을 설정
            PhotonNetwork.LocalPlayer.NickName = GlobalData.nickName;
            //플레이어 이름을 저장
            //PlayerPrefs.SetString("USER_ID", m_UserIDIF.text);

            //인자로 전달된 이름에 해당하는 룸으로 입장
            PhotonNetwork.JoinRoom(roomName);
        }

        GameObject CreateRoomItem(RoomInfo a_RmInfo)
		{
            GameObject room = Instantiate(m_RoomObj);
            room.transform.SetParent(m_ScrollContents.transform, false);

            SGRoomData roomData = room.GetComponent<SGRoomData>();
            roomData.roomName = a_RmInfo.Name;
            roomData.connectPlayer = a_RmInfo.PlayerCount;
            roomData.maxPlayer = a_RmInfo.MaxPlayers;

            roomData.DispRoomData(a_RmInfo.IsOpen);

            return room;
		}
	}
}
