using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enut4LJR
{
    public class FadePanel : MonoBehaviour
    {
        [SerializeField] Image fadeoutImage;
        Color fadeoutColor;
        float alpha;
        bool isFadeOut = false;
        bool isFadeIn = false;
        string gotoScene = "";

        // Start is called before the first frame update
        void Start()
        {
            if (!fadeoutImage) fadeoutImage = GetComponent<Image>();
            fadeoutColor = Color.black;
        }

        // Update is called once per frame
        void Update()
        {
            if (isFadeOut)
			{
                if (alpha < 1.0f)
                {
                    alpha += Time.deltaTime * 0.8f;
                    if (alpha >= 1.0f)
                    {
                        alpha = 1.0f;
                        UnityEngine.SceneManagement.SceneManager.LoadScene(gotoScene);
                        isFadeOut = false;
                    }
                    fadeoutColor.a = alpha;
                    fadeoutImage.color = fadeoutColor;
                }
            }

            if (isFadeIn)
			{
                if (alpha > 0.0f)
				{
                    alpha -= Time.deltaTime * 0.8f;
                    if (alpha <= 0.0f)
					{
                        alpha = 0.0f;
                        isFadeIn = false;
                        gameObject.SetActive(false);
					}
                    fadeoutColor.a = alpha;
                    fadeoutImage.color = fadeoutColor;
                }
			}
            
        }

		//private void OnEnable()
		//{
        //    alpha = 0.0f;
        //    isFadeOut = true;
		//}

        internal void StartFade(int inorOut, string sceneName = "")
		{
            gotoScene = sceneName;
            if (inorOut == 0)
            {
                isFadeIn = true;
                alpha = 1.0f;
            }
            else if (inorOut == 1)
            {
                alpha = 0.0f;
                isFadeOut = true;
            }
            else return;

		}
	}
}
