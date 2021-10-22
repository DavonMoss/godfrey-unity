using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class TargetingState : GodfreyAbstractState
{
    GodfreyStateManager godfrey;

    EnemyManager enemyManager;
    List<float> enemyDistances;
    float enemyDistance;
    bool targeting = false, triggerLockOn = false;
    Transform targetedEnemy;
    CinemachineTargetGroup targetGroup;
    GameObject targetUIInstance;
    int currentEnemyIdx = 0;

    public override void ReceiveInput(InputAction.CallbackContext value)
    {
        if (value.action.name == "LockOn")
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

        if (value.action.name == "LockOnCycle")
        {
            if (value.started)
            {
                cycleTargets();
            }
        }
    }

    public override void EnterState(GodfreyStateManager godfrey)
    {
        this.godfrey = godfrey;

        GameObject enemyManagerObj = GameObject.FindGameObjectWithTag("EnemyManager");
        enemyManager = enemyManagerObj.GetComponent<EnemyManager>();
        targetGroup = enemyManagerObj.GetComponent<CinemachineTargetGroup>();
        enemyDistances = new List<float>();
    }

    public override void UpdateState(GodfreyStateManager godfrey)
    {
        sortEnemies();

        // If we are targeting...
        if (targeting)
        {
            checkRemoveLockOn();
        }
        else if (!targeting && enemyManager.enemiesInSight.Count > 0)
        {
            checkLockOn(triggerLockOn, 0);
        }

        godfrey.setTargeting(targeting, targetedEnemy);
        switchCamLockOnMethod(targeting);
    }

    private void checkLockOn(bool b, int i)
    {
        if (b)
        {
            targetedEnemy = enemyManager.enemiesInSight[i].transform;
            targetGroup.AddMember(targetedEnemy, 1, 4);
            targeting = true;
            triggerLockOn = false;
            applyLockOnUI(targetedEnemy.transform);
        }
    }

    private void checkRemoveLockOn()
    {
        // If enemy dies, stop targeting, and look for next enemy in queue.
        if (targetedEnemy == null)
        {
            targeting = false;

            checkLockOn(true, 0);
        }

        // If we press target button again while targeting, stop targeting.
        if (triggerLockOn)
        {
            targetGroup.RemoveMember(targetedEnemy);
            targeting = false;
            triggerLockOn = false;
            applyLockOnUI(null);
            currentEnemyIdx = 0;
            return;
        }

        // If we move too far away from the enemy, stop targeting.
        Vector3 distCoords = godfrey.transform.position - targetedEnemy.position;
        enemyDistance = Mathf.Sqrt(Mathf.Pow(distCoords.x, 2) + Mathf.Pow(distCoords.y, 2));

        if (enemyDistance > godfrey.lockOnDistance)
        {
            targetGroup.RemoveMember(targetedEnemy);
            applyLockOnUI(null);
            targeting = false;
            currentEnemyIdx = 0;
        }
    }

    private void applyLockOnUI(Transform parent)
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

    private void switchCamLockOnMethod(bool t)
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

    private void sortEnemies()
    {
        for (int i = 1; i < enemyManager.enemiesInSight.Count; i++)
        {
            Transform currentEnemyPos = enemyManager.enemiesInSight[i].transform;
            int j = i - 1;

            while (j >= 0 &&
                (godfrey.transform.position - currentEnemyPos.position).magnitude <
                (godfrey.transform.position - enemyManager.enemiesInSight[j].transform.position).magnitude)
            {
                enemyManager.enemiesInSight[j + 1] = enemyManager.enemiesInSight[j];
                j--;
            }

            enemyManager.enemiesInSight[j + 1] = currentEnemyPos.gameObject;
        }
    }

    private void cycleTargets()
    {
        if (currentEnemyIdx < enemyManager.enemiesInSight.Count - 1)
        {
            currentEnemyIdx++;
        }
        else
        {
            currentEnemyIdx = 0;
        }

        targetGroup.RemoveMember(targetedEnemy);
        applyLockOnUI(null);
        checkLockOn(true, currentEnemyIdx);
    }
}
