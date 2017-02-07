using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public void ProduceSound(ControllableEntity from, BlockController source, float range)
    {
        Collider[] hits = Physics.OverlapSphere(new Vector3(source.X, source.Y, source.transform.position.z), range + 5f);
        foreach(Collider hit in hits)
        {
            ControllableEntity ent = hit.gameObject.GetComponentInChildren<ControllableEntity>();
            if (ent != null && ent != from) 
            {
                Vector3 srcVec = new Vector3(source.X, source.Y, source.transform.position.z);
                RaycastHit[] rayHits = Physics.RaycastAll(ent.transform.parent.position, srcVec, Vector3.Distance(ent.transform.parent.position, srcVec));
                //Debug.DrawLine(ent.transform.parent.position, new Vector3(source.X, source.Y, source.transform.position.z), Color.red, 60.0f);
                float distSound = Vector3.Distance(ent.transform.parent.position, srcVec);
                foreach (RaycastHit rayHit in rayHits)
                {
                    BlockController block = rayHit.collider.GetComponent<BlockController>();
                    if (block.HasObject(e_Object.WALL))
                        distSound--;
                }
                if (distSound >= range + ent.HearingRange)
                    ent.SoundHeard(from, source, distSound);
            }
        }
    }
}
