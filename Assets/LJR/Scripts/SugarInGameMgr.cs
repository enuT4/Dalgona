using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public enum SGState
{
    Ready,
    GameIng,
    Waiting,
    GameEnd,
	StateCount
}

namespace Enut4LJR
{
    public class SugarInGameMgr : MonoBehaviour, ISoundPlay
    {
        public Button m_StartBtn;
        public Text m_TimeTxt;
        public Text m_ProcessTxt;
        public Button m_RestartBtn;

        public float m_Timer = 60.0f;
        public float m_Proc = 0.0f;
        public float m_PrcSpeed = 0.0f;
        public Image m_FatigueGuageImg;
        float m_MaxFtg = 100.0f;
        float m_CurFtg = 0.0f;
        float m_FtgUpSpeed = 20.0f;
        public float m_WarnPoint = 0.0f;
        public float m_DangerPoint = 0.0f;

        public float m_HandSpeed = 0.0f;

        //게임 준비
        public GameObject m_ReadyPanelObj;
        public Text m_ReadyTxt;
        float m_ReadyTimer = 0.0f;
        float m_GoTimer = 0.0f;
        Color m_ReadyColor;

        public static SugarInGameMgr Inst;

        bool isPokeStart = false;
        public GameObject m_PokingHand;

        //달고나 이미지 리스트
        public Sprite[] m_ClearImgList;
        public Sprite[] m_FailImgList;
        //public Image m_DalgonaImg;
        public Sprite[] m_CirPhaseList;
        public Sprite[] m_TriPhaseList;
        public Sprite[] m_StarPhaseList;
        public Sprite[] m_UmbPhaseList;
        DalgonaShape dalgonaShape;

        //달고나 조각
        Image[] Cracks;
        Image[] Fails;
        Vector2 m_ShootVec;
        Vector2[] m_TriDirVec = new Vector2[6];
        Vector2[] m_StarDirVec = new Vector2[5];
        Vector2[] m_UmbDirVec = new Vector2[5];
        Vector2[] m_CirDirVec = new Vector2[4];

        bool isCrackMove = false;
        bool isFailMove = false;

        //결과 기록용 변수
        float m_ResultTime = 0.0f;
        float m_ResultProc = 0.0f;
        string m_ResStr = "";

        //기다리는 페이즈
        public GameObject m_WaitingPanelObj;
        public Text m_WFPTxt;
        public Text m_WaitText;
        string m_DotStr = "";
        float m_WaitTimer = 0.0f;

        //클리어, 게임오버, 시간 초과
        bool isClear = false;
        Color m_OLColor;

        bool isGameOver = false;
        bool isTimeUp = false;
        [SerializeField] GameObject gameSetObj;

        public SGState m_GameState = SGState.StateCount;

        //사운드
        private new AudioSource audio;
        [SerializeField] private AudioClip bgmclip;
        //[SerializeField] private AudioClip crack1clip;
        //[SerializeField] private AudioClip crack2clip;
        private bool isFirstPlay = true;

        ExitGames.Client.Photon.Hashtable playerRecord = new ExitGames.Client.Photon.Hashtable();


        void Awake()
        {
            Inst = this;
            audio = GetComponent<AudioSource>();
            //if (!bgmclip) bgmclip = Resources.Load<AudioClip>("Musics/SGBgm.mp3");
            //if (!crack1clip) crack1clip = Resources.Load<AudioClip>("Sounds/clearcrack1.mp3");
            //if (!crack2clip) crack2clip = Resources.Load<AudioClip>("Sounds/failcrack1.mp3");
            if (!dalgonaShape) dalgonaShape = GameObject.Find("Dalgona").GetComponent<DalgonaShape>();

            InitPlayerRecord();
        }

        // Start is called before the first frame update
        void Start()
        {
            InitPoint();
            SetGroup();

            MusicManager.instance.StopMusic();

            if (m_RestartBtn != null)
                m_RestartBtn.onClick.AddListener(() =>
                {
                    m_PokingHand.SetActive(true);
                    UnityEngine.SceneManagement.SceneManager.LoadScene("SGInGameScene");
                });

            m_ReadyTimer = 2.0f;
            m_ReadyColor = Color.white;
            m_ReadyColor.a = 0.0f;
            m_ReadyTxt.color = m_ReadyColor;

            m_GameState = SGState.Ready;
            if (!gameSetObj) gameSetObj = m_WaitingPanelObj.transform.Find("GameSetObj").gameObject;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_GameState == SGState.Ready)
                ReadyFunc();
            else
                CheckSG();

            if (HandCtrl.Inst.isShake && audio.pitch == 1.0f) audio.pitch = 1.2f;
            else if (!HandCtrl.Inst.isShake && audio.pitch == 1.2f) audio.pitch = 1.0f;


            if (Input.GetKeyDown(KeyCode.G))
			{
                for (int ii = 0; ii < playerRecord.Count; ii++)
				{
                    ADebug.Log(playerRecord["RecordProc"].ToString() + " : " + playerRecord["RecordTime"].ToString());
				}
			}
        }

        void ReadyFunc()
        {
            if (!m_ReadyPanelObj.activeSelf)
                m_ReadyPanelObj.SetActive(true);

            if (m_ReadyTimer > 0.0f)
            {
                m_ReadyTimer -= Time.deltaTime;
                if (m_ReadyTimer <= 0.0f)
                {
                    m_ReadyTimer = 0.0f;
                    m_GoTimer = 1.0f;
                }

                m_ReadyTxt.text = "Ready";
                m_ReadyColor = m_ReadyTxt.color;
                if (m_ReadyColor.a < 1.0f)
                {
                    m_ReadyColor.a = 2 - m_ReadyTimer;
                    if (m_ReadyColor.a >= 1.0f)
                        m_ReadyColor.a = 1.0f;
                }
                m_ReadyTxt.color = m_ReadyColor;
            }

            if (m_GoTimer > 0.0f)
            {
                m_ReadyTxt.text = "Go";
                m_GoTimer -= Time.deltaTime;
                if (m_GoTimer <= 0.0f)
                {
                    m_GoTimer = 0.0f;
                    m_GameState = SGState.GameIng;
                    
                    SoundPlay(ref bgmclip, 0.5f);
                }
            }
        }


        void CheckSG()
        {
            CheckFailProc(DalgonaData.g_DalShape, m_Proc);
            PhaseUpdate(DalgonaData.g_DalShape, m_Proc);
            TimerFunc();
            if (m_GameState == SGState.GameIng)
            {
                if (m_ReadyPanelObj.activeSelf)
                    m_ReadyPanelObj.SetActive(false);
                PokingFunc();
                WarningFunc();
            }
            else if (m_GameState == SGState.Waiting)
            {
                WaitFunc();
                ////데모용
                //if (!m_RestartBtn.gameObject.activeSelf)
                //    m_RestartBtn.gameObject.SetActive(true);
            }
            else if (m_GameState == SGState.GameEnd)
            {
                SendPlayerRecord(m_ResultProc, m_ResultTime);
                //값 임시 저장
                GlobalData.tempRecordProc = m_ResultProc;
                GlobalData.tempRecordTime = m_ResultTime;

                if (m_WFPTxt.gameObject.activeSelf)
                    m_WFPTxt.gameObject.SetActive(false);

                //진행도와 로컬 시간은 실시간으로 체크되고, 여기에서는 위변조의 여부를 판단함
                //모든 유저의 값을 가져오고, 줄을 세운 후에 결과값을 창으로 표시

                gameSetObj.SetActive(true);
                ////다시하기
                //if (!m_RestartBtn.gameObject.activeSelf)
				//{
                //    m_RestartBtn.gameObject.SetActive(true);
                //    audio.Stop();
				//}
                //foreach (Player player in PhotonNetwork.PlayerList)
				//{
                //    float[] eachRecords = ReceivePlayerRecord(player);
                //    ADebug.Log(eachRecords[0] + "% : " + eachRecords[1]);
				//}
            }
        }

        //private void OnGUI()
        //{
        //    GUI.Label(new Rect(36, 30, 2000, 1500), "<color=#ff0000><size=100>" +
        //                            m_Teststr + "</size></color>");
        //}

        void InitPoint()
        {
            m_WarnPoint = Random.Range(50.0f, 55.0f);
            m_DangerPoint = Random.Range(70.0f, 80.0f);

            if (m_RestartBtn.gameObject.activeSelf)
                m_RestartBtn.gameObject.SetActive(false);
        }

        void TimerFunc()
        {
            if (m_Timer > 0.0f)
            {
                m_Timer -= Time.deltaTime;
                if (m_Timer <= 0.0f)
                {
                    m_Timer = 0.0f;
                    if (!isClear && !isGameOver)
                        isTimeUp = true;
                    m_GameState = SGState.Waiting;
                    m_WaitTimer = 4.0f;
                }
            }

            if (0.0f < m_WaitTimer)
            {
                m_WaitTimer -= Time.deltaTime;
                if (m_WaitTimer <= 0.0f)
                {
                    m_WaitTimer = 0.0f;
                    m_GameState = SGState.GameEnd;
                }
            }

            m_TimeTxt.text = "";
            if (m_Timer < 10.0f)
                m_TimeTxt.text += "<color=#ff0000>";
            m_TimeTxt.text += ((int)m_Timer).ToString("D2") + " : " + ((int)((m_Timer - (int)m_Timer) * 100)).ToString("D2");
            if (m_Timer < 10.0f)
                m_TimeTxt.text += "</color>";
        }

        public void PointerDown()
        {
            if (!isPokeStart && !(isClear || isGameOver || isTimeUp))
                isPokeStart = true;
        }

        public void PointerUp()
        {
            if (isPokeStart) isPokeStart = false;
        }

        void PokingFunc()
        {
            if (isClear || isGameOver || isTimeUp) return;

            if (isPokeStart)
            {
                m_Proc += Time.deltaTime * m_PrcSpeed;
                m_CurFtg += Time.deltaTime * m_FtgUpSpeed;
                if (m_CurFtg >= 100.0f) m_CurFtg = 100.0f;
                if (m_Proc >= 100.0f)
                {
                    m_Proc = 100.0f;
                    isClear = true;
                    if (m_PokingHand != null && HandCtrl.Inst.isGo)
                        HandCtrl.Inst.isGo = false;
                    m_GameState = SGState.Waiting;
                }
                else
                {
                    if (m_PokingHand != null)
                        HandCtrl.Inst.isGo = true;
                }
            }
            else
            {
                m_CurFtg -= Time.deltaTime * m_FtgUpSpeed * 2.0f;
                if (m_CurFtg <= 0.0f) m_CurFtg = 0.0f;
                HandCtrl.Inst.isGo = false;
            }
            m_ProcessTxt.text = m_Proc.ToString("F1");
            m_FatigueGuageImg.fillAmount = m_CurFtg / m_MaxFtg;
        }

        void WarningFunc()
        {
            if (m_CurFtg < m_WarnPoint)
            {
                m_PrcSpeed = 3.0f;
                HandCtrl.Inst.isShake = false;
            }
            else
            {
                if (m_DangerPoint <= m_CurFtg)
                {
                    //바로 사망
                    isGameOver = true;
                    GameObject phGroup = GameObject.Find(DalgonaData.g_DalShape.ToString()).transform.GetChild(3).gameObject;
                    if (phGroup.activeSelf) phGroup.SetActive(false);
                    HandCtrl.Inst.isGo = false;
                    m_GameState = SGState.Waiting;
                    SoundManager.instance.PlayerSound("failcrack1", 1.3f);
                    //SoundPlay(ref crack2clip, 1.0f);
                }
                else
                {
                    //경고하는 부분 ( 바늘이 떨리는 연출)
                    HandCtrl.Inst.isShake = true;
                    m_PrcSpeed = 4.5f;
                }
            }
        }

        void PhaseUpdate(Dalgona a_ShapeKind, float a_Proc)
        {
            if (a_ShapeKind == Dalgona.Triangle)
                SetPhase(a_ShapeKind, m_TriDirVec, DalgonaData.g_TriPhase, a_Proc);
            else if (a_ShapeKind == Dalgona.Star)
                SetPhase(a_ShapeKind, m_StarDirVec, DalgonaData.g_StarPhase, a_Proc);
            else if (a_ShapeKind == Dalgona.Umbrella)
                SetPhase(a_ShapeKind, m_UmbDirVec, DalgonaData.g_UmbPhase, a_Proc);
            else if (a_ShapeKind == Dalgona.Circle)
                SetPhase(a_ShapeKind, m_CirDirVec, DalgonaData.g_CirPhase, a_Proc);
        }

        void SetPhase(Dalgona shapeKind, Vector2[] a_DirVec, float[] a_PhaseData, float a_Proc)
        {
            for (int ii = 0; ii < a_PhaseData.Length; ii++)
            {
                if (a_PhaseData[ii] <= a_Proc && a_Proc < a_PhaseData[ii + 1])
                {
                    dalgonaShape.PhaseOnOff(shapeKind, ii);
                    //m_DalgonaImg.sprite = a_PhaseList[ii];
                    //첫번째 파편 떼기
                    if (!Cracks[ii].gameObject.activeSelf)
                    {
                        Cracks[ii].gameObject.SetActive(true);
                        isCrackMove = true;
                        SoundManager.instance.PlayerSound("clearcrack1", 1.3f);
                        //SoundPlay(ref crack1clip, 1.0f);
                    }
                    m_ShootVec = a_DirVec[ii];
                    if (isCrackMove)
                        Cracks[ii].gameObject.transform.Translate(m_ShootVec * Time.deltaTime * 7.0f);
                    if ((Cracks[ii].gameObject.transform.position - dalgonaShape.gameObject.transform.position).magnitude > 35.0f)
                        isCrackMove = false;
                }
            }
        }

        void WaitFunc()
        {
            audio.Stop();
            if (!m_WaitingPanelObj.activeSelf)
            {
                m_ResultProc = m_Proc;
                m_ResultTime = 60 - m_Timer;
                if (isClear)
                {
                    if (isGameOver || isTimeUp) return;
                    m_ResStr = "Game Clear!";
                    m_OLColor = Color.blue;
                }
                if (isGameOver)
                {
                    if (isClear || isTimeUp) return;
                    m_ResStr = "Game Over...";
                    m_OLColor = Color.red;
                }
                if (isTimeUp)
                {
                    if (isClear || isGameOver) return;
                    m_ResStr = "Time Up...";
                    m_OLColor = Color.black;
                }
                m_WaitText.text = m_ResStr + "\n" +
                        "Record : " + m_ResultTime.ToString("F2") + " sec\n" +
                        "Process : " + m_ResultProc.ToString("F1") + " %";

                m_CurFtg = 0.0f;
                m_FatigueGuageImg.fillAmount = m_CurFtg / m_MaxFtg;
                m_WaitText.GetComponent<Outline>().effectColor = m_OLColor;
                m_WaitText.gameObject.SetActive(true);
                m_PokingHand.SetActive(false);
                m_PrcSpeed = 3.0f;
                HandCtrl.Inst.isShake = false;

                m_WaitingPanelObj.SetActive(true);
            }
            if (0.0f < m_Timer) DotFunc(m_Timer);
            else DotFunc(m_WaitTimer);
            if (!m_WFPTxt.gameObject.activeSelf)
                m_WFPTxt.gameObject.SetActive(true);
            m_WFPTxt.text = "다른 플레이어를 기다리는중" + m_DotStr;
        }

        string DotFunc(float a_Timer)
        {
            if ((int)(a_Timer * 2) % 4 == 3) m_DotStr = ".";
            else if ((int)(a_Timer * 2) % 4 == 2) m_DotStr = "..";
            else if ((int)(a_Timer * 2) % 4 == 1) m_DotStr = "...";
            else if ((int)(a_Timer * 2) % 4 == 0) m_DotStr = "....";
            return m_DotStr;
        }

        void SetGroup()
        {
            DalgonaData.SetPhase();
            SetCrackGroup(DalgonaData.g_DalShape);
            SetFailGroup(DalgonaData.g_DalShape);
            SetPointGroup(DalgonaData.g_DalShape);
            SetPhaseDir();
        }

        void SetCrackGroup(Dalgona a_ShapeKind)
        {
            for (int ii = 0; ii < (int)Dalgona.DalgonaCount; ii++)
            {
                if (ii == (int)a_ShapeKind) continue;
                GameObject a_CrackGroup = GameObject.Find(((Dalgona)ii).ToString() + "CrackGroup");
                if (a_CrackGroup) a_CrackGroup.SetActive(false);
            }

            if (GameObject.Find(a_ShapeKind.ToString() + "CrackGroup"))
            {
                Cracks = GameObject.Find(a_ShapeKind.ToString() + "CrackGroup").GetComponentsInChildren<Image>();
                for (int ii = 0; ii < Cracks.Length; ii++)
                    Cracks[ii].gameObject.SetActive(false);
            }
        }

        void SetFailGroup(Dalgona a_ShapeKind)
        {
            for (int ii = 0; ii < (int)Dalgona.DalgonaCount; ii++)
            {
                if (ii == (int)a_ShapeKind) continue;
                GameObject a_FailGroup = GameObject.Find(((Dalgona)ii).ToString() + "FailGroup");
                if (a_FailGroup) a_FailGroup.SetActive(false);
            }

            if (GameObject.Find(a_ShapeKind.ToString() + "FailGroup"))
            {
                Fails = GameObject.Find(a_ShapeKind.ToString() + "FailGroup").GetComponentsInChildren<Image>();
                for (int ii = 0; ii < Fails.Length; ii++)
                    Fails[ii].gameObject.SetActive(false);
            }
        }

        void SetPointGroup(Dalgona a_ShapeKind)
        {
            for (int ii = 0; ii < (int)Dalgona.DalgonaCount; ii++)
            {
                if (ii == (int)a_ShapeKind) continue;
                GameObject a_PointGroup = GameObject.Find(((Dalgona)ii).ToString() + "PointGroup");
                if (a_PointGroup) a_PointGroup.SetActive(false);
            }
        }

        void SetPhaseDir()
        {
            if (DalgonaData.g_DalShape == Dalgona.Triangle)
            {
                m_TriDirVec[0] = new Vector2(-1.0f, 2.0f);
                m_TriDirVec[1] = new Vector2(-2.0f, -1.0f);
                m_TriDirVec[2] = new Vector2(-1.0f, -2.0f);
                m_TriDirVec[3] = new Vector2(1.0f, -2.0f);
                m_TriDirVec[4] = Vector2.right;
                m_TriDirVec[5] = new Vector2(1.0f, 2.5f);
                for (int ii = 0; ii < m_TriDirVec.Length; ii++) m_TriDirVec[ii].Normalize();
            }
            else if (DalgonaData.g_DalShape == Dalgona.Star)
            {
                m_StarDirVec[0] = new Vector2(-1.0f, 2.0f);
                m_StarDirVec[1] = new Vector2(-2.0f, -1.0f);
                m_StarDirVec[2] = Vector2.down;
                m_StarDirVec[3] = new Vector2(2.0f, -1.0f);
                m_StarDirVec[4] = new Vector2(1.0f, 2.0f);
            }
            else if (DalgonaData.g_DalShape == Dalgona.Umbrella)
            {
                m_UmbDirVec[0] = new Vector2(-1.0f, 1.0f);
                m_UmbDirVec[1] = new Vector2(-2.0f, -1.0f);
                m_UmbDirVec[2] = Vector2.down;
                m_UmbDirVec[3] = Vector2.right;
                m_UmbDirVec[4] = new Vector2(1.0f, 2.0f);
            }
            else if (DalgonaData.g_DalShape == Dalgona.Circle)
            {
                m_CirDirVec[0] = new Vector2(-1.0f, 1.0f);
                m_CirDirVec[1] = new Vector2(-1.0f, -2.0f);
                m_CirDirVec[2] = new Vector2(1.0f, -1.0f);
                m_CirDirVec[3] = new Vector2(2.0f, 1.0f);
                for (int ii = 0; ii < m_CirDirVec.Length; ii++) m_CirDirVec[ii].Normalize();
            }
        }

        void CheckFailProc(Dalgona a_ShapeKind, float a_Proc)
        {
            if (!isGameOver) return;

            //if (m_DalgonaImg.gameObject.activeSelf)
            //    m_DalgonaImg.gameObject.SetActive(false);

            if (a_ShapeKind == Dalgona.Triangle)
            {
                if (0 <= a_Proc && a_Proc < DalgonaData.g_TriPhase[0])
                {
                    if (!Fails[0].gameObject.activeSelf || !Fails[1].gameObject.activeSelf)
                    {
                        for (int ii = 0; ii < 2; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);
                            Fails[ii].gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.0f);
                            Fails[ii].gameObject.transform.localPosition = new Vector2(0.0f, -225.0f);
                        }
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        //Fails[0].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * -3.0f);
                        Fails[1].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 3.0f);
                    }

                    if (Fails[1].gameObject.transform.rotation.z >= 0.05f) isFailMove = false;
                }
                else if (DalgonaData.g_TriPhase[0] <= a_Proc && a_Proc < DalgonaData.g_TriPhase[1])
                {
                    if (!Fails[2].gameObject.activeSelf || !Fails[3].gameObject.activeSelf || !Fails[4].gameObject.activeSelf)
                    {
                        for (int ii = 2; ii < 5; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);
                            if (ii != 4)
                            {
                                Fails[ii].gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.0f);
                                Fails[ii].gameObject.transform.localPosition = new Vector2(0.0f, -225.0f);
                            }

                        }
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[2].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * -2.0f);
                        Fails[3].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 5.0f);
                        Fails[4].gameObject.transform.Translate(Vector2.left.normalized * Time.deltaTime * 5.0f);
                    }

                    if (Fails[3].gameObject.transform.rotation.z >= 0.05f) isFailMove = false;
                }
                else if (DalgonaData.g_TriPhase[1] <= a_Proc && a_Proc < DalgonaData.g_TriPhase[2])
                {
                    if (!Fails[5].gameObject.activeSelf || !Fails[6].gameObject.activeSelf || !Fails[7].gameObject.activeSelf)
                    {
                        for (int ii = 5; ii < 8; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);
                            if (ii != 7)
                            {
                                Fails[ii].gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1.0f);
                                Fails[ii].gameObject.transform.localPosition = new Vector2(0.0f, 225.0f);
                            }

                        }
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[6].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 3.0f);
                        Fails[7].gameObject.transform.Translate(new Vector2(-1.0f, -3.0f).normalized * Time.deltaTime * 5.0f);
                    }

                    if (Fails[6].gameObject.transform.rotation.z >= 0.05f) isFailMove = false;
                }
                else if (DalgonaData.g_TriPhase[2] <= a_Proc && a_Proc < DalgonaData.g_TriPhase[3])
                {
                    if (!Fails[8].gameObject.activeSelf || !Fails[9].gameObject.activeSelf ||
                        !Fails[10].gameObject.activeSelf || !Fails[11].gameObject.activeSelf)
                    {
                        for (int ii = 8; ii < 12; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);
                            if (ii == 8)
                            {
                                Fails[ii].gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.0f);
                                Fails[ii].gameObject.transform.localPosition = new Vector2(0.0f, -225.0f);
                            }
                            else if (ii == 10)
                            {
                                Fails[ii].gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1.0f);
                                Fails[ii].gameObject.transform.localPosition = new Vector2(0.0f, 225.0f);
                            }


                        }
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[8].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 2.0f);
                        Fails[9].gameObject.transform.Translate(new Vector2(-1.0f, 3.0f).normalized * Time.deltaTime * 3.0f);
                        Fails[10].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 3.0f);
                        Fails[11].gameObject.transform.Translate(new Vector2(2.0f, -1.0f).normalized * Time.deltaTime * 5.0f);
                    }

                    if ((Fails[11].gameObject.transform.position - dalgonaShape.gameObject.transform.position).magnitude > 10.0f) isFailMove = false;
                }
                else if (DalgonaData.g_TriPhase[3] <= a_Proc && a_Proc < DalgonaData.g_TriPhase[4])
                {
                    if (!Fails[12].gameObject.activeSelf || !Fails[13].gameObject.activeSelf || !Fails[14].gameObject.activeSelf)
                    {
                        for (int ii = 12; ii < 15; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);
                            if (ii == 13)
                            {
                                Fails[ii].gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 0.5f);
                                Fails[ii].gameObject.transform.localPosition = new Vector2(-225.0f, 0.0f);
                            }
                            else if (ii == 14)
                            {
                                Fails[ii].gameObject.GetComponent<RectTransform>().pivot = new Vector2(1.0f, 0.5f);
                                Fails[ii].gameObject.transform.localPosition = new Vector2(225.0f, 0.0f);
                            }

                        }
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[12].gameObject.transform.Translate(new Vector2(-1.0f, 3.0f).normalized * Time.deltaTime * 2.0f);
                        Fails[13].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 1.5f);
                        Fails[14].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 3.0f);
                    }

                    if (Fails[14].gameObject.transform.rotation.z >= 0.05f) isFailMove = false;
                }
                else if (DalgonaData.g_TriPhase[4] <= a_Proc && a_Proc < DalgonaData.g_TriPhase[5])
                {
                    if (!Fails[15].gameObject.activeSelf || !Fails[16].gameObject.activeSelf || !Fails[17].gameObject.activeSelf)
                    {
                        for (int ii = 15; ii < 18; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);
                        }
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[15].gameObject.transform.Translate(new Vector2(1.0f, 3.0f).normalized * Time.deltaTime * 4.0f);
                        Fails[16].gameObject.transform.Translate(Vector2.down.normalized * Time.deltaTime * 1.0f);
                        Fails[17].gameObject.transform.Translate(new Vector2(-1.0f, 2.0f).normalized * Time.deltaTime * 2.5f);
                    }

                    if ((Fails[17].gameObject.transform.position - dalgonaShape.gameObject.transform.position).magnitude > 8.0f) isFailMove = false;
                }

            }
            else if (a_ShapeKind == Dalgona.Star)
            {
                if (0 <= a_Proc && a_Proc < DalgonaData.g_StarPhase[0])
                {
                    if (!Fails[0].gameObject.activeSelf || !Fails[1].gameObject.activeSelf || !Fails[2].gameObject.activeSelf)
                    {
                        for (int ii = 0; ii < 3; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);

                        }
                        Fails[1].gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.0f);
                        Fails[1].gameObject.transform.localPosition = new Vector2(0.0f, -225.0f);
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[1].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 3.0f);
                        Fails[2].gameObject.transform.Translate(new Vector2(-5.0f, -1.0f).normalized * Time.deltaTime * 10.0f);
                    }

                    if (Fails[1].gameObject.transform.rotation.z >= 0.05f) isFailMove = false;
                }
                else if (DalgonaData.g_StarPhase[0] <= a_Proc && a_Proc < DalgonaData.g_StarPhase[1])
                {
                    if (!Fails[3].gameObject.activeSelf || !Fails[4].gameObject.activeSelf || !Fails[5].gameObject.activeSelf)
                    {
                        for (int ii = 3; ii < 6; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);
                            if (ii != 5)
                            {
                                Fails[ii].gameObject.GetComponent<RectTransform>().pivot = new Vector2(1.0f, 0.5f);
                                Fails[ii].gameObject.transform.localPosition = new Vector2(225.0f, 0.0f);
                            }
                        }
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[3].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * -2.0f);
                        Fails[4].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 2.0f);
                    }

                    if (Fails[4].gameObject.transform.rotation.z >= 0.035f) isFailMove = false;
                }
                else if (DalgonaData.g_StarPhase[1] <= a_Proc && a_Proc < DalgonaData.g_StarPhase[2])
                {
                    if (!Fails[6].gameObject.activeSelf || !Fails[7].gameObject.activeSelf || !Fails[8].gameObject.activeSelf)
                    {
                        for (int ii = 6; ii < 9; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);

                        }
                        Fails[7].gameObject.GetComponent<RectTransform>().pivot = new Vector2(1.0f, 1.0f);
                        Fails[7].gameObject.transform.localPosition = new Vector2(225.0f, 225.0f);
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[6].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * -1.5f);
                        Fails[6].gameObject.transform.Translate(new Vector2(1.0f, 5.0f).normalized * Time.deltaTime * 10.0f);
                        Fails[7].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 2.0f);
                    }

                    if (Fails[7].gameObject.transform.rotation.z >= 0.035f) isFailMove = false;
                }
                else if (DalgonaData.g_StarPhase[2] <= a_Proc && a_Proc < DalgonaData.g_StarPhase[3])
                {
                    if (!Fails[9].gameObject.activeSelf || !Fails[10].gameObject.activeSelf || !Fails[11].gameObject.activeSelf)
                    {
                        for (int ii = 9; ii < 12; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);
                            if (ii != 10)
                            {
                                Fails[ii].gameObject.GetComponent<RectTransform>().pivot = new Vector2(1.0f, 1.0f);
                                Fails[ii].gameObject.transform.localPosition = new Vector2(225.0f, 225.0f);
                            }
                        }
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[9].gameObject.transform.Translate(new Vector2(-1.0f, -1.0f).normalized * Time.deltaTime * 3.0f);
                        Fails[10].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * -3.0f);
                        Fails[10].gameObject.transform.Translate(Vector2.up.normalized * Time.deltaTime * 10.0f);
                        Fails[11].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 3.0f);
                    }

                    if (Fails[11].gameObject.transform.rotation.z >= 0.05f) isFailMove = false;
                }
                else if (DalgonaData.g_StarPhase[3] <= a_Proc && a_Proc < DalgonaData.g_StarPhase[4])
                {
                    if (!Fails[12].gameObject.activeSelf || !Fails[13].gameObject.activeSelf || !Fails[14].gameObject.activeSelf)
                    {
                        for (int ii = 12; ii < 15; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);
                        }
                        Fails[12].gameObject.GetComponent<RectTransform>().pivot = new Vector2(1.0f, 0.5f);
                        Fails[12].gameObject.transform.localPosition = new Vector2(225.0f, 0.0f);
                        Fails[14].gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1.0f);
                        Fails[14].gameObject.transform.localPosition = new Vector2(0.0f, 225.0f);
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[12].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 2.0f);
                        Fails[13].gameObject.transform.Translate(new Vector2(-5.0f, 1.0f).normalized * Time.deltaTime * 5.0f);
                        Fails[14].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 3.0f);
                    }

                    if (Fails[14].gameObject.transform.rotation.z >= 0.05f) isFailMove = false;
                }
            }
            else if (a_ShapeKind == Dalgona.Umbrella)
            {
                if (0 <= a_Proc && a_Proc < DalgonaData.g_UmbPhase[0])
                {
                    if (!Fails[0].gameObject.activeSelf || !Fails[1].gameObject.activeSelf || !Fails[2].gameObject.activeSelf)
                    {
                        for (int ii = 0; ii < 3; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);

                        }
                        Fails[1].gameObject.GetComponent<RectTransform>().pivot = new Vector2(1.0f, 0.5f);
                        Fails[1].gameObject.transform.localPosition = new Vector2(225.0f, 0.0f);
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[1].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 2.0f);
                        Fails[2].gameObject.transform.Translate(Vector2.left.normalized * Time.deltaTime * 10.0f);
                    }

                    if (Fails[1].gameObject.transform.rotation.z >= 0.035f) isFailMove = false;
                }
                else if (DalgonaData.g_UmbPhase[0] <= a_Proc && a_Proc < DalgonaData.g_UmbPhase[1])
                {
                    if (!Fails[3].gameObject.activeSelf || !Fails[4].gameObject.activeSelf || !Fails[5].gameObject.activeSelf)
                    {
                        for (int ii = 3; ii < 6; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);

                        }
                        Fails[4].gameObject.GetComponent<RectTransform>().pivot = new Vector2(1.0f, 0.5f);
                        Fails[4].gameObject.transform.localPosition = new Vector2(225.0f, 0.0f);
                        Fails[5].gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.0f);
                        Fails[5].gameObject.transform.localPosition = new Vector2(0.0f, -225.0f);
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[4].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 2.0f);
                        Fails[5].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 3.0f);
                    }

                    if (Fails[5].gameObject.transform.rotation.z >= 0.05f) isFailMove = false;
                }
                else if (DalgonaData.g_UmbPhase[1] <= a_Proc && a_Proc < DalgonaData.g_UmbPhase[2])
                {
                    if (!Fails[6].gameObject.activeSelf || !Fails[7].gameObject.activeSelf || !Fails[8].gameObject.activeSelf)
                    {
                        for (int ii = 6; ii < 9; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);
                            if (ii != 6)
                            {
                                Fails[ii].gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.0f);
                                Fails[ii].gameObject.transform.localPosition = new Vector2(0.0f, -225.0f);
                            }
                        }
                        Fails[6].gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1.0f);
                        Fails[6].gameObject.transform.localPosition = new Vector2(0.0f, 225.0f);
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[6].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 3.0f);
                        Fails[7].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 3.0f);
                        Fails[8].gameObject.transform.Translate(Vector2.up.normalized * Time.deltaTime * 10.0f);
                        Fails[8].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * -10.0f);
                    }

                    if (Fails[7].gameObject.transform.rotation.z >= 0.05f) isFailMove = false;
                }
                else if (DalgonaData.g_UmbPhase[2] <= a_Proc && a_Proc < DalgonaData.g_UmbPhase[3])
                {
                    if (!Fails[9].gameObject.activeSelf || !Fails[10].gameObject.activeSelf ||
                        !Fails[11].gameObject.activeSelf || !Fails[12].gameObject.activeSelf)
                    {
                        for (int ii = 9; ii < 13; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);
                        }
                        Fails[9].gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1.0f);
                        Fails[9].gameObject.transform.localPosition = new Vector2(0.0f, 225.0f);
                        Fails[12].gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.0f);
                        Fails[12].gameObject.transform.localPosition = new Vector2(0.0f, -225.0f);
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[9].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 3.0f);
                        Fails[11].gameObject.transform.Translate(new Vector2(-1.0f, 2.0f).normalized * Time.deltaTime * 10.0f);
                        Fails[12].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 2.0f);
                    }

                    if (Fails[9].gameObject.transform.rotation.z >= 0.05f) isFailMove = false;
                }
                else if (DalgonaData.g_UmbPhase[3] <= a_Proc && a_Proc < DalgonaData.g_UmbPhase[4])
                {
                    if (!Fails[13].gameObject.activeSelf || !Fails[14].gameObject.activeSelf ||
                        !Fails[15].gameObject.activeSelf || !Fails[16].gameObject.activeSelf)
                    {
                        for (int ii = 13; ii < 17; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);
                        }
                        Fails[13].gameObject.GetComponent<RectTransform>().pivot = new Vector2(1.0f, 1.0f);
                        Fails[13].gameObject.transform.localPosition = new Vector2(225.0f, 225.0f);
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[13].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 3.0f);
                        Fails[14].gameObject.transform.Translate(new Vector2(-1.0f, 1.0f).normalized * Time.deltaTime * 10.0f);
                        Fails[15].gameObject.transform.Translate(new Vector2(-1.0f, 3.0f).normalized * Time.deltaTime * 10.0f);
                        Fails[16].gameObject.transform.Translate(new Vector2(-1.0f, 1.0f).normalized * Time.deltaTime * 5.0f);
                    }

                    if (Fails[13].gameObject.transform.rotation.z >= 0.05f) isFailMove = false;
                }

            }
            else if (a_ShapeKind == Dalgona.Circle)
            {
                if (0 <= a_Proc && a_Proc < DalgonaData.g_CirPhase[0])
                {
                    if (!Fails[0].gameObject.activeSelf || !Fails[1].gameObject.activeSelf || !Fails[2].gameObject.activeSelf)
                    {
                        for (int ii = 0; ii < 3; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);
                            if (ii == 2) continue;
                            Fails[ii].gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.0f);
                            Fails[ii].gameObject.transform.localPosition = new Vector2(0.0f, -225.0f);
                        }
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[0].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * -3.0f);
                        Fails[1].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 3.0f);
                        Fails[2].gameObject.transform.Translate(new Vector2(-1.0f, 3.0f).normalized * Time.deltaTime * 5.0f);
                    }

                    if (Fails[0].gameObject.transform.rotation.z <= -0.05f) isFailMove = false;
                }
                else if (DalgonaData.g_CirPhase[0] <= a_Proc && a_Proc < DalgonaData.g_CirPhase[1])
                {
                    if (!Fails[3].gameObject.activeSelf || !Fails[4].gameObject.activeSelf ||
                        !Fails[5].gameObject.activeSelf || !Fails[6].gameObject.activeSelf)
                    {
                        for (int ii = 3; ii < 7; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);
                        }
                        Fails[4].gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.0f);
                        Fails[4].gameObject.transform.localPosition = new Vector2(0.0f, -225.0f);
                        Fails[6].gameObject.GetComponent<RectTransform>().pivot = new Vector2(1.0f, 0.5f);
                        Fails[6].gameObject.transform.localPosition = new Vector2(225.0f, 0.0f);
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[3].gameObject.transform.Translate(Vector2.up.normalized * Time.deltaTime * 3.0f);
                        Fails[4].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 3.0f);
                        Fails[5].gameObject.transform.Translate(new Vector2(2.0f, -1.0f).normalized * Time.deltaTime * 1.3f);
                        Fails[6].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 3.0f);
                    }

                    if (Fails[4].gameObject.transform.rotation.z >= 0.05f) isFailMove = false;
                }
                else if (DalgonaData.g_CirPhase[1] <= a_Proc && a_Proc < DalgonaData.g_CirPhase[2])
                {
                    if (!Fails[7].gameObject.activeSelf || !Fails[8].gameObject.activeSelf || !Fails[9].gameObject.activeSelf)
                    {
                        for (int ii = 7; ii < 10; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);
                        }
                        Fails[9].gameObject.GetComponent<RectTransform>().pivot = new Vector2(1.0f, 0.5f);
                        Fails[9].gameObject.transform.localPosition = new Vector2(225.0f, 0.0f);
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[7].gameObject.transform.Translate(new Vector2(-1.0f, 1.0f).normalized * Time.deltaTime * 3.0f);
                        Fails[8].gameObject.transform.Translate(Vector2.up.normalized * Time.deltaTime * 5.0f);
                        Fails[9].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 3.0f);
                    }

                    if (Fails[9].gameObject.transform.rotation.z >= 0.05f) isFailMove = false;
                }
                else if (DalgonaData.g_CirPhase[2] <= a_Proc && a_Proc < DalgonaData.g_CirPhase[3])
                {
                    if (!Fails[10].gameObject.activeSelf || !Fails[11].gameObject.activeSelf || !Fails[12].gameObject.activeSelf)
                    {
                        for (int ii = 10; ii < 13; ii++)
                        {
                            Fails[ii].gameObject.SetActive(true);
                        }
                        Fails[12].gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1.0f);
                        Fails[12].gameObject.transform.localPosition = new Vector2(0.0f, 225.0f);
                        isFailMove = true;
                    }

                    if (isFailMove)
                    {
                        Fails[11].gameObject.transform.Translate(Vector2.up.normalized * Time.deltaTime * 5.0f);
                        Fails[12].gameObject.transform.Rotate(0.0f, 0.0f, Time.deltaTime * 2.0f);
                    }

                    if (Fails[12].gameObject.transform.rotation.z >= 0.05f) isFailMove = false;
                }
            }
        }//void CheckFailProc(Dalgona a_ShapeKind, float a_Proc)

        void InitPlayerRecord()
		{
            if (PhotonNetwork.CurrentRoom == null)
                return;

            playerRecord.Clear();
            playerRecord.Add("RecordProc", 0.0f);
            playerRecord.Add("RecordTime", 0.0f);
            PhotonNetwork.CurrentRoom.SetCustomProperties(playerRecord);
		}

        void SendPlayerRecord(float process, float time)
		{
            if (playerRecord == null)
			{
                playerRecord = new ExitGames.Client.Photon.Hashtable();
                playerRecord.Clear();
			}

            if (playerRecord.ContainsKey("RecordProc"))
                playerRecord["RecordProc"] = process;
            else
                playerRecord.Add("RecordProc", process);

            if (playerRecord.ContainsKey("RecordTime"))
                playerRecord["RecordTime"] = time;
            else
                playerRecord.Add("RecordTime", time);

            PhotonNetwork.LocalPlayer.SetCustomProperties(playerRecord);
        }

        float[] ReceivePlayerRecord(Player player)
		{
            float[] playerRecord = new float[2] { 0.0f, 0.0f };

            if (player == null) 
                return playerRecord;

            if (player.CustomProperties.ContainsKey("RecordProc"))
                playerRecord[0] = (float)player.CustomProperties["RecordProc"];

            if (player.CustomProperties.ContainsKey("RecordTime"))
                playerRecord[1] = (float)player.CustomProperties["RecordTime"];

            return playerRecord;
		}

        public void SoundPlay(ref AudioClip clip, float volume)
		{
            if (audio == null) return;
            if (GlobalData.masterMute || GlobalData.musicMute) return;
            //audio.Stop();

            audio.PlayOneShot(clip, volume);
		}
    }
}

