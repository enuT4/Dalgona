using UnityEngine;

namespace Altair.Utility
{
    public static class Utility
    {
        /// <summary>
        /// Component 변수가 null일 경우, Hierarchy에서 Object를 검색해 Component를 가져오는데 성공할 경우 true를 반환하는 boolean형 함수
        /// </summary>
        /// <typeparam name="T">Compnent Type</typeparam>
        /// <param name="name">Object 이름</param>
        /// <param name="result">out으로 가져올 인자</param>
        /// <returns></returns>
        public static bool TryGetComponentSafety<T>(string name, out T result) where T : MonoBehaviour
        {
            result = default(T);
            GameObject obj = null;
            obj = GameObject.Find(name);

            if (obj != null && obj.TryGetComponent(out result))
                return true;

            return false;
        }

        /// <summary>
        /// Component 변수가 null일 경우, Hierarchy에서 Object를 검색해 Component를 가져오는데 실패할 경우 Component를 추가하는 boolean형 함수
        /// </summary>
        /// <typeparam name="T">Compnent Type</typeparam>
        /// <param name="name">Object 이름</param>
        /// <param name="result">out으로 가져올 인자</param>
        /// <returns></returns>
        public static bool TryAddComponentSafety<T>(string name, out T result) where T : MonoBehaviour
        {
            result = default(T);
            GameObject obj = null;
            obj = GameObject.Find(name);

            if (obj != null)
            {
                if (obj.TryGetComponent(out result))
                    return true;
                else
                {
                    result = obj.AddComponent<T>();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Component 변수가 null일 경우, Hierarchy에서 Object를 검색해 Object가 없을 경우 Object를 생성해 Component를 추가하는 MonoBehaviour형 함수
        /// </summary>
        /// <typeparam name="T">Compnent Type</typeparam>
        /// <param name="name">Object 이름</param>
        /// <returns></returns>
        public static T TryInstantiateComponentSafety<T>(string name) where T : MonoBehaviour
        {
            T result = default(T);
            GameObject obj = null;
            obj = GameObject.Find(name);

            if (obj != null)
            {
                if (obj.TryGetComponent(out result))
                    return result;
                else
                {
                    result = obj.AddComponent<T>();
                    return result;
                }
            }
            else
            {
                obj = new GameObject(name);
                result = obj.AddComponent<T>();
                return result;
            }
        }
    }
}
