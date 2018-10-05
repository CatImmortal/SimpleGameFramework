using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Procedure;
using SimpleGameFramework.Fsm;

public class Procedure_1st : ProcedureBase {

    public override void OnUpdate(Fsm<ProcedureManager> fsm, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);
        if (Input.GetMouseButtonDown(0))
        {
            ChangeState<Procedure_2nd>(fsm);
        }
    }


}
