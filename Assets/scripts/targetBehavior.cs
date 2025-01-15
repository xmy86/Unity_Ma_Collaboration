using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetBehavior : MonoBehaviour
{
    public void InitializePosition()
    {
        transform.localPosition = new Vector3(Random.Range(-3f, 3f), 0.3f, Random.Range(-3f, 3f));
    }
    
}
