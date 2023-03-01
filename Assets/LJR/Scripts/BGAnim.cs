using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enut4LJR
{
    public class BGAnim : MemoryPoolObject
    {
        Vector2 startPos;
        Vector2 endPos;
        Vector2 tempPos;
        [SerializeField]float value = 0.0f;

        SGTitleMgr titleManager;


        // Start is called before the first frame update
        void Start()
        {
            if (!titleManager) titleManager = GameObject.Find("TitleMgr").GetComponent<SGTitleMgr>();

            startPos = transform.localPosition;
            endPos = new Vector2(startPos.x + 700.0f, startPos.y - 700.0f);
        }

        // Update is called once per frame
        void Update()
        {
            if (value < 1.0f)
			{
                value += Time.deltaTime * 0.2f;
                if (value >= 1.0f)
				{
                    value = 1.0f;
                    ObjectReturn();
                    titleManager.SpawnBG(startPos);
				}
            }
            tempPos = Vector2.Lerp(startPos, endPos, value);
            transform.localPosition = tempPos;
        }


		private void OnEnable()
		{
            transform.localPosition = startPos;
            value = 0.0f;
        }
	}
}
