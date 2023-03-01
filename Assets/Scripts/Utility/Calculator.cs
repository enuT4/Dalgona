using UnityEngine;

namespace Altair.Calculator
{
    public static class Calculator
    {
        /// <summary>
        /// 2D Object �̵� ������ ���� ��ǥ �������� �����̰� �ϴ� �Լ�. �⺻���� Y��
        /// </summary>
        /// <param name="transform">��� Object�� Transform</param>
        /// <param name="x">x���� �������� �� ��� true</param>
        /// <param name="y">y���� �������� �� ��� true</param>
        /// <returns></returns>
        public static Vector2 LocalVector2(Transform transform, bool x = false, bool y = true)
        {
            Vector2 localVector = transform.InverseTransformDirection(transform.position);

            if (x)
                localVector.x = 1;
            else
                localVector.x = 0;

            if(y)
                localVector.y = 1;
            else
                localVector.y = 0;

            return transform.TransformDirection(localVector);
        }

        /// <summary>
        /// 2D ȯ�濡�� LookAt�� �����ϴ� Quaternion�� �Լ�
        /// </summary>
        /// <param name="origin">��� Object�� Transform</param>
        /// <param name="target">Ÿ�� Object�� Transform</param>
        /// <returns></returns>
        public static Quaternion LookAt2D(Transform origin, Transform target)
        {
            Vector2 direction = origin.position - target.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion angleAxis = Quaternion.AngleAxis(angle + 90f, Vector3.forward);
            return angleAxis;
        }

        /// <summary>
        /// 2D ȯ�濡�� LookAt�� �����ϴ� Vector3�� �Լ�
        /// </summary>
        /// <param name="origin">��� Object�� Transform</param>
        /// <param name="target">Ÿ�� Object�� Transform</param>
        /// <param name="vector3">�Լ� �����ε�� ��¥ �Ű�����</param>
        /// <returns></returns>
        public static Vector3 LookAt2D(Transform origin, Transform target , bool vector3 = true)
        {
            Vector3 Pos = target.position;
            Pos.z = origin.position.z;
            Pos.Normalize();

            return Pos;
        }
    }
}
