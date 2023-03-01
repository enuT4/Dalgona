using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Enut4LJR
{
    public enum SGScene
	{
        SG_Ready,
        SG_Play,
        SG_Result,
	}

	public class SGReadyMgr : MonoBehaviourPunCallbacks
    {
        public Button m_ExitBtn;
        public Button m_GameStartBtn;
        public Button m_ReadyBtn;

        private PhotonView pv;


        //게임 채팅
        bool bEnter = false;
        public InputField m_MsgIF;
        public Text m_LogMsgTxt;

        //게임 정보
        public Image m_GKImg;
        public Text m_GKText;

        public GameObject m_UserNodeItem;
        public GameObject m_UserScroll;

        public static SGScene m_SceneState;

        [SerializeField] GameObject fadeObj;
        FadePanel fadePanel;
        bool isGameStart;


        ExitGames.Client.Photon.Hashtable m_SceneProps = new ExitGames.Client.Photon.Hashtable();

        ExitGames.Client.Photon.Hashtable m_PlayerReady = new ExitGames.Client.Photon.Hashtable();

		void Awake()
		{
            m_SceneState = SGScene.SG_Ready;

            pv = GetComponent<PhotonView>();

            PhotonNetwork.IsMessageQueueRunning = true;

            GetConnectPlayerCount();

            InitReadyProps();
            InitSGSceneProps();

            if (!fadeObj) fadeObj = GameObject.Find("Canvas").transform.Find("FadeOutPanel").gameObject;
            if (!fadePanel) fadePanel = fadeObj.GetComponent<FadePanel>();
		}

		// Start is called before the first frame update
		void Start()
        {
            if (m_ExitBtn != null)
                m_ExitBtn.onClick.AddListener(OnClickExitRoom);

            if (m_ReadyBtn != null)
                m_ReadyBtn.onClick.AddListener(() =>
                {
                    SendReady(1);
                });

            string msg = "\n<color=#00ff00>[" +
                PhotonNetwork.LocalPlayer.NickName +
                "] Connected</color>";
            
            //pv.RPC("LogMsg", RpcTarget.AllBuffered, msg);

            if (m_GameStartBtn != null)
                m_GameStartBtn.onClick.AddListener(() =>
                {
                    SendSGScene(SGScene.SG_Play);        //게임 씬으로 이동하도록
                });

            fadeObj.SetActive(true);
            fadePanel.StartFade(0);
        }

        public void OnApplicationFocus(bool focus)
        {
            SGPhotonInit.isFocus = focus;
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsGamePossible()) return;

            if (m_SceneState == SGScene.SG_Ready)
			{
                if (IsDifferentList())
                    RefreshReadyState();
			}



            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
                bEnter = !bEnter;
                if (bEnter)
				{
                    m_MsgIF.gameObject.SetActive(true);
                    m_MsgIF.ActivateInputField();
				}
                else
				{
                    m_MsgIF.gameObject.SetActive(false);
                    if (m_MsgIF.text != "")
					{
                        EnterText();
					}
				}
			}

            AllReadyObserver();

            if (m_SceneState == SGScene.SG_Play && !isGameStart)
			{
                fadeObj.SetActive(true);
                fadePanel.StartFade(1, "SGInGameScene");
                m_SceneState = SGScene.SG_Ready;
                isGameStart = true;
			}


        }

        void EnterText()
        {
            string msg = "\n<color=#ffffff>[" +
                PhotonNetwork.LocalPlayer.NickName +
                "] " + m_MsgIF.text + "</color>";
            //pv.RPC("LogMsg", RpcTarget.AllBuffered, msg);

            m_MsgIF.text = "";
        }

        void GetConnectPlayerCount()
		{
            Room currRoom = PhotonNetwork.CurrentRoom;
            ADebug.Log(currRoom.PlayerCount.ToString()
                          + "/"
                          + currRoom.MaxPlayers.ToString());
            //룸 접속자 정보 조회 함수
		}

        public override void OnPlayerEnteredRoom(Player a_Player)
		{
            GetConnectPlayerCount();
		}

        public override void OnPlayerLeftRoom(Player a_Player)
		{
            GetConnectPlayerCount();
		}

        public void OnClickExitRoom()
		{
            string msg = "\n<color=#ff0000>[" + 
                PhotonNetwork.LocalPlayer.NickName +
                "] Disconnected</color>";

            //pv.RPC("LogMsg", RpcTarget.AllBuffered, msg);

            if (PhotonNetwork.PlayerList != null && PhotonNetwork.PlayerList.Length <= 1)
			{
                if (PhotonNetwork.CurrentRoom != null)
                    PhotonNetwork.CurrentRoom.CustomProperties.Clear();
			}

            if (PhotonNetwork.LocalPlayer != null)
                PhotonNetwork.LocalPlayer.CustomProperties.Clear();

            PhotonNetwork.LeaveRoom();
		}

        public override void OnLeftRoom()
		{
            //fadeObj.SetActive(true);
            //fadePanel.StartFade(1, "SGLobbyScene");
            SceneManager.LoadScene("SGLobbyScene");
		}

        [PunRPC]
        void LogMsg(string msg)
        {
            m_LogMsgTxt.text += msg;
        }

        bool IsDifferentList()
		{
            GameObject[] userNodes = GameObject.FindGameObjectsWithTag("UserInfoNode");

            if (userNodes == null) return true;

            if (PhotonNetwork.PlayerList.Length != userNodes.Length) return true;

            foreach (Player a_RefPlayer in PhotonNetwork.PlayerList)
			{
                bool isFindNode = false;
                SGUserData userData = null;
                foreach (GameObject a_Node in userNodes)
				{
                    userData = a_Node.GetComponent<SGUserData>();
                    if (userData == null) continue;

                    if (userData.m_UniqueID == a_RefPlayer.ActorNumber)
					{
                        if (userData.m_IamReady != ReceiveReady(a_RefPlayer))
                            return true;

                        isFindNode = true;
                        break;
					}
                    if (!isFindNode) return true;
				}
			}
            return false;
        }

        void RefreshReadyState()
		{
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("UserInfoNode"))
                Destroy(obj);


            GameObject a_UserNode = null;
            //PhotonNetwork.PlayerList.Length
            foreach (Player a_RefPlayer in PhotonNetwork.PlayerList)
			{
                a_UserNode = Instantiate(m_UserNodeItem);
                a_UserNode.transform.SetParent(m_UserScroll.transform, false);
                SGUserData userData = a_UserNode.GetComponent<SGUserData>();
                if (userData != null)
				{
                    userData.m_UniqueID = a_RefPlayer.ActorNumber;
                    userData.m_IamReady = ReceiveReady(a_RefPlayer);
                    bool a_isMine = (userData.m_UniqueID == PhotonNetwork.LocalPlayer.ActorNumber);
                    userData.DispUserData(a_isMine, a_RefPlayer.NickName);
				}
			}

            if (ReceiveReady(PhotonNetwork.LocalPlayer))
			{
				m_ExitBtn.interactable = false;
                m_ReadyBtn.gameObject.SetActive(false);
			}
			else
			{
				m_ExitBtn.interactable = true;
                m_ReadyBtn.gameObject.SetActive(true);
			}
        }

        bool IsGamePossible()
		{
            if (PhotonNetwork.CurrentRoom == null ||
                PhotonNetwork.LocalPlayer == null)
                return false;

            if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SGScene"))
                return false;

            m_SceneState = ReceiveSGScene();

            return true;
        }

        #region --------------Scene 상태 동기화 처리

        void InitSGSceneProps()
		{
            if (PhotonNetwork.CurrentRoom == null) return;

            m_SceneProps.Clear();
            m_SceneProps.Add("SGScene", SGScene.SG_Ready);
            PhotonNetwork.CurrentRoom.SetCustomProperties(m_SceneProps);
		}

        void SendSGScene(SGScene a_SGScene)
		{
            if (m_SceneProps == null)
			{
                m_SceneProps = new ExitGames.Client.Photon.Hashtable();
                m_SceneProps.Clear();
			}

            if (m_SceneProps.ContainsKey("SGScene"))
                m_SceneProps["SGScene"] = (int)a_SGScene;
            else
                m_SceneProps.Add("SGScene", (int)a_SGScene);

            PhotonNetwork.CurrentRoom.SetCustomProperties(m_SceneProps);
        }

        SGScene ReceiveSGScene()
		{
            SGScene a_RmVal = SGScene.SG_Ready;

            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SGScene"))
                a_RmVal = (SGScene)PhotonNetwork.CurrentRoom.CustomProperties["SGScene"];

            return a_RmVal;
		}

        #endregion  //--------------Scene 상태 동기화 처리

        #region --------------Ready 상태 동기화 처리

        void InitReadyProps()
		{
            m_PlayerReady.Clear();
            m_PlayerReady.Add("IamReady", 0);
            PhotonNetwork.LocalPlayer.SetCustomProperties(m_PlayerReady);
		}

        void SendReady(int a_Ready = 1)
		{
            if (m_PlayerReady == null)
			{
                m_PlayerReady = new ExitGames.Client.Photon.Hashtable();
                m_PlayerReady.Clear();
			}

            if (m_PlayerReady.ContainsKey("IamReady"))
                m_PlayerReady["IamReady"] = a_Ready;

            PhotonNetwork.LocalPlayer.SetCustomProperties(m_PlayerReady);
		}

        bool ReceiveReady(Player a_Player)
        {
            if (a_Player == null)
                return false;

            if (!a_Player.CustomProperties.ContainsKey("IamReady"))
                return false;

            if ((int)a_Player.CustomProperties["IamReady"] == 1)
                return true;

            return false;
        }

        #endregion //--------------Ready 상태 동기화 처리


        void AllReadyObserver()
		{
            //레디상태만 확인하도록 예외처리

            //int a_OldGoWait = (int) ->??

            bool a_AllReady = true;
            foreach (Player a_RefPlayer in PhotonNetwork.PlayerList)
			{
                if (!ReceiveReady(a_RefPlayer))
				{
                    a_AllReady = false;
                    break;
				}
			}

            if (a_AllReady)
			{
                if (PhotonNetwork.CurrentRoom.IsOpen)
				{
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    //PhotonNetwork.CurrentRoom.IsVisible = false;
				}
                if (PhotonNetwork.IsMasterClient)
                    m_GameStartBtn.gameObject.SetActive(true);

            }
		}
    }
}
