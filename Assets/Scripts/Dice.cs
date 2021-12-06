using System.Text.RegularExpressions;
using UnityEngine;
using System;
public class Dice
{
    public static int Roll(string formula)
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
            accu += UnityEngine.Random.Range(0, sides)+1;
        }
        return accu + bonus;
    }
}