using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enut4LJR
{
    public class SGTitleMgr : MonoBehaviour
    {
        //배경 무한 움직이는 효과
        [SerializeField] GameObject bgObj;
        Vector2[] spawnPos = new Vector2[4];
        float prefabSize = 1400.0f;//1255.0f;

        void Awake() => AwakeFunc();

		private void AwakeFunc()
		{
            if (!SoundManager.instance) SoundManager.instance.CallInstance();
            if (!MusicManager.instance) MusicManager.instance.CallInstance();
		}

		// Start is called before the first frame update
		void Start()
        {
            MusicManager.instance.PlayMusic("BGM1");
            //배경 무한 움직이는 효과
            InitPos();
            InitSpawnBG();
        }

        //void Update() => UpdateFunc();
        
        // Update is called once per frame
        void UpdateFunc()
        {
            if (Input.GetKeyDown(KeyCode.G))
			{
                ADebug.Log(Screen.width + " : " + Screen.height);
			}
        }

        #region 배경 무한 움직이는 효과

        void InitPos()
		{
            spawnPos[0] = new Vector2(0.5f, 0.0f) * prefabSize;
            spawnPos[1] = new Vector2(-0.5f, 0.0f) * prefabSize;
            spawnPos[2] = new Vector2(-0.5f, 1.0f) * prefabSize;
            spawnPos[3] = new Vector2(0.5f, 1.0f) * prefabSize;
        }

        void InitSpawnBG()
		{
            for (int ii = 0; ii < spawnPos.Length; ii++)
            {
                bgObj = MemoryPoolManager.instance.GetObject("ImageParent");
                bgObj.transform.position = spawnPos[ii];
                bgObj.SetActive(true);
            }
        }

        internal void SpawnBG(Vector2 spawnPos)
		{
            bgObj = MemoryPoolManager.instance.GetObject("ImageParent");
            bgObj.transform.position = spawnPos;
            bgObj.SetActive(true);
        }

		#endregion


        

    }
}
