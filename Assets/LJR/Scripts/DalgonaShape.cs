using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enut4LJR
{
    public class DalgonaShape : MonoBehaviour
    {
        //달고나 모양 정하기
        int randShapeInt = -1;
        //[SerializeField] Image dalgonaImg;
        [SerializeField] GameObject[] dalgonaList;

        //데모 버전
        bool isDemo = false;
        int demoInt = -1;

		private void Awake()
		{
            InitDalList();
            ChooseShape();
		}

		// Start is called before the first frame update
		void Start()
        {
        
        }
        
        // Update is called once per frame
        void Update()
        {
        
        }
        
        void InitDalList()
		{
            if (!dalgonaList[0]) dalgonaList[0] = transform.Find("Triangle").gameObject;
            if (!dalgonaList[1]) dalgonaList[1] = transform.Find("Star").gameObject;
            if (!dalgonaList[2]) dalgonaList[2] = transform.Find("Umbrella").gameObject;
            if (!dalgonaList[3]) dalgonaList[3] = transform.Find("Circle").gameObject;

            for (int ii = 0; ii < dalgonaList.Length; ii++)
			{
                if (dalgonaList[ii].activeSelf) dalgonaList[ii].SetActive(false);
			}
		}

        void ChooseShape()
        {
            if (isDemo) randShapeInt = DemoShapeInt();
            else randShapeInt = Random.Range(0, (int)Dalgona.DalgonaCount);
            //randShapeInt = 0;
            DalgonaData.g_DalShape = (Dalgona)randShapeInt;

            dalgonaList[randShapeInt].SetActive(true);
            //dalgonaImg.sprite = dalgonaList[randShapeInt];
            //isLL = true;
        }

        int DemoShapeInt()
        {
            demoInt++;
            if (demoInt >= (int)Dalgona.DalgonaCount)
                demoInt -= (int)Dalgona.DalgonaCount;
            ADebug.Log(demoInt);
            return demoInt;
        }

        public void PhaseOnOff(Dalgona a_ShapeKind, int offIdx)
		{
            if (dalgonaList[(int)a_ShapeKind].transform.Find("PhaseGroup").GetChild(offIdx).gameObject.activeSelf)
            {
                dalgonaList[(int)a_ShapeKind].transform.Find("PhaseGroup").GetChild(offIdx).gameObject.SetActive(false);
                dalgonaList[(int)a_ShapeKind].transform.Find("PhaseGroup").GetChild(offIdx + 1).gameObject.SetActive(true);
            }

            //if (a_ShapeKind == Dalgona.Triangle)
			//{
            //    if (dalgonaList[0].transform.Find("PhaseGroup").GetChild(offIdx).gameObject.activeSelf)
			//	{
            //        dalgonaList[0].transform.Find("PhaseGroup").GetChild(offIdx).gameObject.SetActive(false);
            //        dalgonaList[0].transform.Find("PhaseGroup").GetChild(offIdx + 1).gameObject.SetActive(true);
            //    }
            //
            //}
			//else if (a_ShapeKind == Dalgona.Star)
			//{
            //
			//}
            //else if (a_ShapeKind == Dalgona.Umbrella)
			//{
            //
			//}
            //else if (a_ShapeKind == Dalgona.Circle)
			//{
            //
			//}


		}
	}
}
