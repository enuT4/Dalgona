using UnityEngine;

public class MemoryPoolObject : MonoBehaviour
{
    [HideInInspector] public int listIdx = 0;
    [HideInInspector] public int itemNumber = 0;
    public void ObjectReturn()
    {
        MemoryPoolManager.instance.ObjectReturn(listIdx, itemNumber);
    }

    public virtual void InitObject() { }
}