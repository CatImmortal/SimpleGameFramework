using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Procedure;
using SimpleGameFramework.Fsm;

public class Procedure_2nd : ProcedureBase {



    public override void OnUpdate(Fsm<ProcedureManager> fsm, float elapseSeconds, float realElapseSeconds)
    {
        if (Input.GetMouseButtonDown(0))
        {
            ChangeState<Procedure_3rd>(fsm);
        }
    }
}
