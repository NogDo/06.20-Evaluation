using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortal : MonoBehaviour
{
    #region public º¯¼ö
    public Transform targetPosition;
    #endregion

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CPlayerController>(out CPlayerController controller))
        {
            controller.Teleport(targetPosition.position);
        }
    }
}
