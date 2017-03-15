using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    private TurnManager _tManager;

    void Awake()
    {
        _tManager = GetComponent<TurnManager>();
    }

    public void ProduceSound(ControllableEntity from, BlockController source, float range)
    {
        List<ControllableEntity> players = _tManager.GetAllEntities();
        Vector3 srcVec = new Vector3(source.X, source.Y, source.transform.position.z);
        foreach (ControllableEntity ent in players)
        { 
            if (ent != from)
            {
                RaycastHit[] rayHits = Physics.RaycastAll(ent.transform.parent.position, srcVec - ent.transform.parent.position, Vector3.Distance(ent.transform.parent.position, srcVec));
                //Debug.DrawRay(ent.transform.parent.position, srcVec - ent.transform.parent.position, Color.red, 10.0f);
                float distSound = Vector3.Distance(ent.transform.parent.position, srcVec);
                foreach (RaycastHit rayHit in rayHits)
                {
                    BlockController block = rayHit.collider.GetComponent<BlockController>();
                    if (block.HasObject(e_Object.WALL))
                        distSound++;
                }
                //Debug.Log("range = " + range);
                if (Mathf.Floor(distSound) <= range + ent.HearingRange)
                    ent.SoundHeard(from, source, distSound);
            }
        }
    }
}
