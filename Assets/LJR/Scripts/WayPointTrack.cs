using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointTrack : MonoBehaviour
{
	public Color lineColor = Color.blue;
    private Transform[] points;

	private void OnDrawGizmos()
	{
        //������ ���� ����
        Gizmos.color = lineColor;
        //WayPointGroup ���ӿ�����Ʈ �Ʒ��� �ִ� ��� Point ���ӿ�����Ʈ ����
        points = GetComponentsInChildren<Transform>();

        int nextIdx = 1;

        Vector3 currPos = points[nextIdx].position;
        Vector3 nextPos;

        //for (int ii = 0; ii < points.Length; ii++)
		//{
        //    Gizmos.DrawSphere(points[ii].transform.position, 5.0f);
        //}

        //Point ���ӿ�����Ʈ�� ��ȸ�ϸ鼭 ������ �׸�
        for (int i = 0; i <= points.Length; i++)
        {
            //������ Point�� ��� ù ��° point�� ����
            nextPos = (++nextIdx >= points.Length) ?
                points[1].position : points[nextIdx].position;
            //���� ��ġ���� ���� ��ġ���� ������ �׸�
            Gizmos.DrawLine(currPos, nextPos);
            currPos = nextPos;
        }
    }

	//// Start is called before the first frame update
	//void Start()
	//{
	//    
	//}
	//
	//// Update is called once per frame
	//void Update()
	//{
	//    
	//}
}
