using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enut4LJR
{
    public class HandCtrl : MonoBehaviour
    {
        float m_MoveSpeed = 0.1f;

        private Transform tr;

        Transform[] points;
        public int nextIdx = 2;
        public bool isGo = false;

        public static HandCtrl Inst;
        Vector2 direction = Vector2.zero;

        //�ް� ��� ���ϱ�
        //public Dalgona m_ShapeKind = Dalgona.DalgonaCount;
        int m_RandShapeInt = -1;
        public Image m_DalgomaImg;
        public Sprite[] m_DalgonaList;
        public static int m_DemoInt = 0;

        //�ް� ���� ���ϱ�
        int idx = 1;
        float m_LL = 0.0f;
        bool isLL = false;
        Vector3 currPos;
        Vector3 nextPos;
        public float m_TotalLength;

        //������
        public Image m_HandImg;
        Vector3 m_ImgPos;
        Vector3 m_TempPos;
        public bool isShake = false;

        void Awake()
        {
            Inst = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            //ChooseShape();
            CheckShape(DalgonaData.g_DalShape);
            LineLengthFunc();

            m_ImgPos = m_HandImg.gameObject.transform.localPosition;
        }

        // Update is called once per frame
        void Update()
        {
            if (isGo)
                m_MoveSpeed = (SugarInGameMgr.Inst.m_PrcSpeed * m_TotalLength) / 100.0f;

            HandMove();

            if (!isShake)
                m_HandImg.gameObject.transform.localPosition = m_ImgPos;
            else
                HandShake();

            if (Input.GetKeyDown(KeyCode.G))
            {
                ADebug.Log(m_HandImg.gameObject.transform.localPosition);
            }
        }

        void HandMove()
        {
            if (!isGo) return;
            direction = points[nextIdx].position - tr.position;
            direction.Normalize();
            tr.Translate(direction * Time.deltaTime * m_MoveSpeed);
            //ADebug.Log((points[nextIdx].position - tr.position).magnitude + " : " + SugarInGameMgr.Inst.m_PrcSpeed + " : " + m_TotalLength);
            if ((points[nextIdx].position - tr.position).magnitude < 0.8f)
            {
                //ADebug.Log(nextIdx + " : " + SugarInGameMgr.Inst.m_Proc);
                nextIdx = (++nextIdx >= points.Length) ? 1 : nextIdx;
            }
        }

        void HandShake()
        {
            if (!isShake) return;

            m_TempPos = m_ImgPos;
            m_TempPos.x += Random.Range(-3.0f, 3.0f);
            m_TempPos.y += Random.Range(-3.0f, 3.0f);
            m_HandImg.gameObject.transform.localPosition = m_TempPos;
        }

        //void ChooseShape()
        //{
        //    m_RandShapeInt = Random.Range(0, (int)Dalgona.DalgonaCount);
        //    //m_RandShapeInt = DemoShapeInt();
        //    m_ShapeKind = (Dalgona)m_RandShapeInt;
        //    //m_DemoInt++;
        //    m_DalgomaImg.sprite = m_DalgonaList[m_RandShapeInt];
        //    isLL = true;
        //}

        //int DemoShapeInt()
        //{
        //    if (m_DemoInt >= 4)
        //        m_DemoInt -= 4;
        //    ADebug.Log(m_DemoInt);
        //    return m_DemoInt;
        //}

        void CheckShape(Dalgona a_ShapeKind)
        {
            ADebug.Log(a_ShapeKind);
            points = GameObject.Find(a_ShapeKind.ToString() + "PointGroup").GetComponentsInChildren<Transform>();
            tr = GetComponent<Transform>();
            tr.transform.position = points[1].position;
            direction = points[nextIdx].position - tr.position;
            direction.Normalize();
            isLL = true;
        }

        void LineLengthFunc()
        {
            currPos = points[idx].position;

            //Point ���ӿ�����Ʈ�� ��ȸ�ϸ鼭 ������ �׸�
            for (int i = 0; i <= points.Length; i++)
            {
                //������ Point�� ��� ù ��° point�� ����
                nextPos = (++idx >= points.Length) ? points[1].position :
                    points[idx].position;
                //���� ��ġ���� ���� ��ġ���� ������ �׸�
                if (isLL)
                    m_LL += (nextPos - currPos).magnitude;
                currPos = nextPos;
                if (i == points.Length && isLL)
                {
                    m_TotalLength = m_LL;
                    isLL = false;
                }
            }
        }

    }

}
