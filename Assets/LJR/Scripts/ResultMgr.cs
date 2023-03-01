using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Enut4LJR
{
    enum ResultState
	{
        Success,
        Failure,
        Timeup,
        ResultCount
	}

    public class DalgonaRecord
	{
        public string userID { get; set; }
        public string nickName { get; set; }
        public float recordProcess { get; set; } = 0.0f;
        public float recordTime { get; set; } = 0.0f;
	}

	public class ResultMgr : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Text winnerText;
        [SerializeField] private Button lobbyBtn;
        [SerializeField] private Button readyBtn;

        [SerializeField] private GameObject fadeObj;
        FadePanel panel;

        List<DalgonaRecord> recordList = new List<DalgonaRecord>();

        bool isGG = false;
        [SerializeField] private Text myGoldTxt;
        [SerializeField]private float tempGold;
        private int resultGold;
        float goldSpeed = 100.0f;

        [SerializeField] private GameObject rankingObj;
        GameObject tempRankObj;
        [SerializeField] private Transform rankingScroll;

        ExitGames.Client.Photon.Hashtable playerRecord = new ExitGames.Client.Photon.Hashtable();

        void Awake() => AwakeFunc();

        void AwakeFunc()
		{
            if (!rankingScroll) rankingScroll = GameObject.Find("Content").transform;
            if (!winnerText) winnerText = GameObject.Find("Canvas").transform.Find("WinnerText").GetComponent<Text>();
            if (!lobbyBtn) lobbyBtn = GameObject.Find("Canvas").transform.Find("GotoLobbyBtn").GetComponent<Button>();
            if (!readyBtn) readyBtn = GameObject.Find("Canvas").transform.Find("GotoReadyBtn").GetComponent<Button>();
            if (!fadeObj) fadeObj = GameObject.Find("Canvas").transform.Find("FadeOutPanel").gameObject;
            if (!myGoldTxt) myGoldTxt = GameObject.Find("MyInfoImage").transform.Find("MoneyText").GetComponent<Text>();
            if (!panel) panel = fadeObj.GetComponent<FadePanel>();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (lobbyBtn != null)
                lobbyBtn.onClick.AddListener(OnClickExitRoom);
            if (readyBtn != null)
                readyBtn.onClick.AddListener(() => GotoScene("SGReadyScene"));

            SortUserRecords();

            if (fadeObj != null)
                fadeObj.SetActive(true);
            panel.StartFade(0);

            SoundManager.instance.PlayerSound("result1");

        }

        void Update() => UpdateFunc();

        // Update is called once per frame
        void UpdateFunc()
        {
            //if (Input.GetKeyDown(KeyCode.G))
			//{
            //    ADebug.Log(playerRecord["RecordTime"]);
            //    ADebug.Log(playerRecord["RecordProc"]);
			//}

            if (isGG)
			{
                if (GlobalData.nickName == recordList[0].nickName)
				{
                    if (tempGold < resultGold)
                    {
                        tempGold += Time.deltaTime * goldSpeed;
                        if (tempGold >= resultGold)
                        {
                            tempGold = resultGold;
                            isGG = false;
                            GlobalData.myGold = resultGold;
                        }

                        myGoldTxt.text = ((int)tempGold).ToString();
                    }
                }
                else
				{
                    if (tempGold > resultGold)
					{
                        tempGold -= Time.deltaTime * goldSpeed;
                        if (tempGold <= resultGold)
						{
                            tempGold = resultGold;
                            isGG = false;
                            GlobalData.myGold = resultGold;
						}


                        myGoldTxt.text = ((int)tempGold).ToString();
					}
				}
                
			}
        }


        void SortUserRecords()
		{
            // 점수와 기록을 불러온 후에 이를 줄세우는 과정
            if (recordList != null)
                recordList.Clear();

            foreach (var item in PhotonNetwork.PlayerList)
            {
                DalgonaRecord record = new DalgonaRecord();
                record.userID = item.UserId;
                record.nickName = item.NickName;
                if (item.CustomProperties.ContainsKey("RecordProc"))
                    record.recordProcess = (float)item.CustomProperties["RecordProc"];
                if (item.CustomProperties.ContainsKey("RecordTime"))
                    record.recordTime = (float)item.CustomProperties["RecordTime"];
                recordList.Add(record);
            }

            recordList.Sort(SortwithProc);

            PrintRankingBoard();
        }   

        int SortwithProc(DalgonaRecord record1, DalgonaRecord record2)
		{
            if (record1.recordProcess != record2.recordProcess)
                return -record1.recordProcess.CompareTo(record2.recordProcess);
            else
                return record1.recordTime.CompareTo(record2.recordTime);
		}

        void PrintRankingBoard()
		{
            for (int ii = 0; ii < recordList.Count; ii++)
            {
                if (tempRankObj != null) tempRankObj = null;
                tempRankObj = Instantiate(rankingObj);
                tempRankObj.transform.SetParent(rankingScroll, false);
                RankingObj tempRank = tempRankObj.GetComponent<RankingObj>();
                tempRank.DisplayRanking(recordList[ii].nickName, ii + 1, recordList[ii].recordProcess, recordList[ii].recordTime);


                //if (GlobalData.userID == recordList[ii].userID)
                //    resultText.text += "<color=#ff0000>";
                //resultText.text += recordList[ii].nickName + " : " + recordList[ii].recordProcess.ToString("F2") + "%, " + recordList[ii].recordTime.ToString("F2") + "초\n";
                //if (GlobalData.userID == recordList[ii].userID)
                //    resultText.text += "</color>";
            }

            if (string.IsNullOrEmpty(winnerText.text)) winnerText.text = "";
            winnerText.text = recordList[0].nickName + " 승리!!!";
            DistributeGold();
        }

        void GotoScene(string sceneName)
		{
            fadeObj.SetActive(true);
            panel.StartFade(1, sceneName);
        }

        void DistributeGold()
		{
            tempGold = GlobalData.myGold;

            if (GlobalData.nickName == recordList[0].nickName)
			{
				resultGold = GlobalData.myGold + 100 * (recordList.Count - 1);
                goldSpeed *= recordList.Count - 1;
			}
			else
                resultGold = GlobalData.myGold - 100;

            isGG = true;
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
    }
}
