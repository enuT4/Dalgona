using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Default
{
    public class GameSet : MonoBehaviour
    {
        //[SerializeField] private Image gameSetImg;
        //[SerializeField] private Text gameSetTxt;
        Vector2 startPos;
        Vector2 endPos;
        Vector2 tempPos;
        float value = 1.0f;

        WaitForSeconds twoWFS = new WaitForSeconds(2.0f);

        //void Awake() => AwakeFunc();

        void AwakeFunc()
		{
            
        }

        // Start is called before the first frame update
        void Start()
        {
            startPos = new Vector2(1500.0f, 0.0f);
            endPos = Vector2.zero;
            //if (gameSetImg != null) gameSetImg.gameObject.transform.localPosition = startPos;
            transform.position = startPos;
        }

        // Update is called once per frame
        void Update()
        {
            if (value < 1.0f)
            {
                value += Time.deltaTime * 3.0f;
                if (value >= 1.0f)
                {
                    value = 1.0f;
                    //텍스트 출력
                    StartCoroutine(DelayTime());
                }
                tempPos = Vector2.Lerp(startPos, endPos, value);
                transform.localPosition = tempPos;
            }
        }

		private void OnEnable()
		{
            transform.localPosition = startPos;
            value = 0.0f;
		}

        IEnumerator DelayTime()
		{
            yield return twoWFS;
            SceneManager.LoadScene("SGResultScene");
		}
	}
}
