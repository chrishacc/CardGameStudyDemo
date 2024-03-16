using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardUtils
{
    public static bool CardHasTargetableEffect(CardTemplate card)
    {
        //�жϿ����Ƿ�Ҫչʾ������ͷ���жϱ�׼�ǿ����Ƿ�����Թ������˵���Ч
        foreach(var effect in card.Effects)
        {
            if (effect is TargetableEffect targetableEffect)
            {
                if(targetableEffect.Target == EffectTargetType.TargetEnemy)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
