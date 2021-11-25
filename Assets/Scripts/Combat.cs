using System;
using System.Text.RegularExpressions;
using UnityEngine;
public class Combat
{
    private System.Random rng = new System.Random();

    public int Attack(Entity attacker, Entity defender)
    {
        int roll = rollDice("1d20");
        bool attackSuccessful = roll >= (attacker.Thac0 - defender.ArmorClass);
        if (attackSuccessful)
        {
            int damage = rollDice(attacker.Damage);
            defender.HitPoints = defender.HitPoints - damage;
            return damage;
        }
        return 0;
    }

    private int rollDice(string formula)
    {
        int bonus = 0;
        int sides = 0;
        int dices = 0;
        int accu = 0;
        // Formula examples:
        //  3d4+12
        //  3d6
        Regex rx = new Regex(@"(\d+)[d](\d+)[+]?(\d*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        MatchCollection matches = rx.Matches(formula);
        foreach (Match match in matches)
        {
            GroupCollection groups = match.Groups;
            dices = Int32.Parse(groups[1].Value);
            sides = Int32.Parse(groups[2].Value);
            if (groups[3].Value != null && groups[3].Value != "")
            {
                bonus = Int32.Parse(groups[3].Value);
            }
        }
        for (int i = 0; i < dices; i++) {
            accu += rng.Next(sides)+1;
        }
        return accu + bonus;
    }
}