using System;
using System.Text.RegularExpressions;
using UnityEngine;
public class Combat
{
    public static int Attack(Entity attacker, Entity defender)
    {
        int roll = Dice.Roll("1d20");
        bool attackSuccessful = roll >= (attacker.Thac0 - defender.ArmorClass);
        if (attackSuccessful)
        {
            int damage = Dice.Roll(attacker.Damage);
            defender.HitPoints = defender.HitPoints - damage;
            return damage;
        }
        return 0;
    }
}