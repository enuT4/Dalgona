using UnityEngine;

namespace Altair.Calculator
{
    public static class Calculator
    {
        /// <summary>
        /// 2D Object 이동 방향을 로컬 좌표 기준으로 움직이게 하는 함수. 기본형은 Y축
        /// </summary>
        /// <param name="transform">대상 Object의 Transform</param>
        /// <param name="x">x축을 기준으로 할 경우 true</param>
        /// <param name="y">y축을 기준으로 할 경우 true</param>
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
        /// 2D 환경에서 LookAt을 구현하는 Quaternion형 함수
        /// </summary>
        /// <param name="origin">대상 Object의 Transform</param>
        /// <param name="target">타깃 Object의 Transform</param>
        /// <returns></returns>
        public static Quaternion LookAt2D(Transform origin, Transform target)
        {
            Vector2 direction = origin.position - target.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion angleAxis = Quaternion.AngleAxis(angle + 90f, Vector3.forward);
            return angleAxis;
        }

        /// <summary>
        /// 2D 환경에서 LookAt을 구현하는 Vector3형 함수
        /// </summary>
        /// <param name="origin">대상 Object의 Transform</param>
        /// <param name="target">타깃 Object의 Transform</param>
        /// <param name="vector3">함수 오버로드용 가짜 매개변수</param>
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
