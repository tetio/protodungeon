public class Combat
{
    private System.Random rng = new System.Random();

    public int Attack(Entity attacker, Entity victim)
    {
        int roll = rollDice(20);
        bool attackSuccessful = roll >= (attacker.Thac0 - victim.ArmorClass);
        if (attackSuccessful)
        {
            int damage = rollDice(attacker.Damage);
            victim.HitPoints = victim.HitPoints - damage;
            return damage;
        }
        return 0;
    }

    private int rollDice(int sides) 
    {
        return rng.Next(sides)+1;
    }

}