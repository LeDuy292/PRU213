using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_BorealController : MonoBehaviour
{
    // ...existing variables...

    private void Update()
    {
        if (isDie || player == null) return;

        float deltaX = player.position.x - transform.position.x;
        float distance = Mathf.Abs(deltaX);
        int dir = deltaX > 0 ? 1 : -1;

        Flip(dir);

        // 🔒 ĐANG DÙNG SKILL
        if (currentSkill != SkillState.None)
        {
            HandleSkill();
            UpdateAnimation();
            return;
        }

        // 🔴 ƯU TIÊN 1: CẬN CHIẾN (player rất gần)
        if (distance <= meleeRange)
        {
            moveX = 0;
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackDelay &&
                Time.time >= lastMeleeTime + meleeCooldown)
            {
                UseMeleeSkill();
            }

            UpdateAnimation();
            return;
        }

        // 🔵 ƯU TIÊN 2: BẮN XA (player ở tầm bắn, nhưng không quá gần)
        if (distance > meleeRange && distance <= shootRange)
        {
            moveX = 0; // Đứng yên để bắn

            if (Time.time >= lastShootSkillTime + shootSkillCooldown)
            {
                UseShootSkill();
            }

            UpdateAnimation();
            return;
        }

        // 🏃 NGOÀI TẦM BẮN → ĐUỔI THEO
        moveX = dir * moveSpeed;
        attackTimer = 0;

        UpdateAnimation();
    }

    // ...existing methods...
}