using UnityEngine;

namespace Altair.Utility
{
    public static class Utility
    {
        /// <summary>
        /// Component ������ null�� ���, Hierarchy���� Object�� �˻��� Component�� �������µ� ������ ��� true�� ��ȯ�ϴ� boolean�� �Լ�
        /// </summary>
        /// <typeparam name="T">Compnent Type</typeparam>
        /// <param name="name">Object �̸�</param>
        /// <param name="result">out���� ������ ����</param>
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
        /// Component ������ null�� ���, Hierarchy���� Object�� �˻��� Component�� �������µ� ������ ��� Component�� �߰��ϴ� boolean�� �Լ�
        /// </summary>
        /// <typeparam name="T">Compnent Type</typeparam>
        /// <param name="name">Object �̸�</param>
        /// <param name="result">out���� ������ ����</param>
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
        /// Component ������ null�� ���, Hierarchy���� Object�� �˻��� Object�� ���� ��� Object�� ������ Component�� �߰��ϴ� MonoBehaviour�� �Լ�
        /// </summary>
        /// <typeparam name="T">Compnent Type</typeparam>
        /// <param name="name">Object �̸�</param>
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
