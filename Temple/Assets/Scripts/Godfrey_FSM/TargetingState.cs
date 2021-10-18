using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class TargetingState : GodfreyAbstractState
{
    EnemyManager enemyManager;
    List<float> enemyDistances;
    float enemyDistance;
    bool targeting = false, triggerLockOn = false;
    Transform targetedEnemy;
    CinemachineTargetGroup targetGroup;
    GameObject targetUIInstance;

    public override void ReceiveInput(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            triggerLockOn = true;
        }
        else if (value.canceled)
        {
            triggerLockOn = false;
        }
    }

    public override void EnterState(GodfreyStateManager godfrey)
    {
        GameObject enemyManagerObj = GameObject.FindGameObjectWithTag("EnemyManager");
        enemyManager = enemyManagerObj.GetComponent<EnemyManager>();
        targetGroup = enemyManagerObj.GetComponent<CinemachineTargetGroup>();
        enemyDistances = new List<float>();
    }

    public override void UpdateState(GodfreyStateManager godfrey)
    {
        // If we are targeting...
        if (targeting)
        {
            checkRemoveLockOn(godfrey);
        }
        else if (!targeting && enemyManager.enemiesInSight.Count > 0)
        {
            calcEnemyDistances(godfrey);
            checkLockOn(godfrey);
        }

        godfrey.setTargeting(targeting, targetedEnemy);
        switchCamLockOnMethod(godfrey, targeting);
    }

    private void calcEnemyDistances(GodfreyStateManager godfrey)
    {
        enemyDistances = new List<float>();

        foreach (GameObject enemy in enemyManager.enemiesInSight)
        {
            Vector3 distCoords = godfrey.transform.position - enemy.transform.position;
            enemyDistances.Add(Mathf.Sqrt(Mathf.Pow(distCoords.x, 2) + Mathf.Pow(distCoords.y, 2)));
        }
    }

    private void checkLockOn(GodfreyStateManager godfrey)
    {
        if (triggerLockOn)
        {
            float minDist = enemyDistances.Min();
            int minDistIdx = enemyDistances.IndexOf(minDist);

            targetedEnemy = enemyManager.enemiesInSight[minDistIdx].transform;
            targetGroup.AddMember(targetedEnemy, 1, 4);
            targeting = true;
            triggerLockOn = false;
            applyLockOnUI(godfrey, targetedEnemy.transform);
            return;
        }
    }

    private void checkRemoveLockOn(GodfreyStateManager godfrey)
    {
        // If enemy dies, stop targeting.
        if (targetedEnemy == null)
        {
            targeting = false;
        }

        // If we press target button again while targeting, stop targeting.
        if (triggerLockOn)
        {
            targetGroup.RemoveMember(targetedEnemy);
            targeting = false;
            triggerLockOn = false;
            applyLockOnUI(godfrey, null);
            return;
        }

        // If we move too far away from the enemy, stop targeting.
        Vector3 distCoords = godfrey.transform.position - targetedEnemy.position;
        enemyDistance = Mathf.Sqrt(Mathf.Pow(distCoords.x, 2) + Mathf.Pow(distCoords.y, 2));

        if (enemyDistance > godfrey.lockOnDistance)
        {
            targetGroup.RemoveMember(targetedEnemy);
            applyLockOnUI(godfrey, null);
            targeting = false;
        }
    }

    private void applyLockOnUI(GodfreyStateManager godfrey, Transform parent)
    {
        if (parent != null)
        {
            targetUIInstance = GameObject.Instantiate(godfrey.targetUI);
            targetUIInstance.transform.SetParent(parent, false);
        }
        else
        {
            GameObject.Destroy(targetUIInstance);
        }
    }

    private void switchCamLockOnMethod(GodfreyStateManager godfrey, bool t)
    {
        if (t)
        {
            godfrey.lockOnCam.Priority = 1;
            godfrey.freeLookCam.Priority = 0;
        }
        else
        {
            godfrey.freeLookCam.Priority = 1;
            godfrey.lockOnCam.Priority = 0;
        }
    }
}
