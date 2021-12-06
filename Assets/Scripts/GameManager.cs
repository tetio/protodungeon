using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    private Hero hero;

    private float duration = 0.3f;

    int layerMask = ~(1 << 10);

    private int score;

    private Dungeon dungeon;

    private InputSource inputSource;

    AudioSource sound;

    private System.Random rng = new System.Random();

    private bool isCoroutineBusy = false;

    Vector3 heroDestPosition;
    public List<GameObject> Mobs
    {
        get {return dungeon.Mobs;}
    }

    // Start is called before the first frame update
    void Awake()
    {
        dungeon = GameObject.Find("/Dungeon").GetComponent<Dungeon>();
        hero = GameObject.Find("/Hero").GetComponent<Hero>();
        inputSource = GameObject.Find("/Canvas/Panel").GetComponent<InputSource>();

        sound = GetComponent<AudioSource>();
        sound.clip = hero.getAudioClipFootStep();

        dungeon.generateDungeon(1);
    }

void Update()
    {
        if (isCoroutineBusy)
        {
            return;
        }
        float vertical = inputSource.Direction.z;
        float horizontal = inputSource.Direction.x;
        GameObject goHero = hero.transform.gameObject;
        bool heroIsMoving = (vertical != 0 || horizontal != 0);

        if (Math.Abs(vertical) > Math.Abs(horizontal) && vertical > 0)
        {
            heroDestPosition = hero.transform.position + Vector3.up;
            if (CanMove(goHero, heroDestPosition))
                StartCoroutine(LerpPosition(goHero, heroDestPosition, duration)); //will do the lerp over two seconds
        }
        else if (Math.Abs(vertical) > Math.Abs(horizontal) && vertical < -0)
        {
            heroDestPosition = hero.transform.position + Vector3.down;
            if (CanMove(goHero, heroDestPosition))
                StartCoroutine(LerpPosition(goHero, heroDestPosition, duration)); //will do the lerp over two seconds
        }
        else if (Math.Abs(vertical) < Math.Abs(horizontal) && horizontal > 0)
        {
            heroDestPosition = hero.transform.position + Vector3.right;
            if (CanMove(goHero, heroDestPosition))
                StartCoroutine(LerpPosition(goHero, heroDestPosition, duration)); //will do the lerp over two seconds
        }
        else if (Math.Abs(vertical) < Math.Abs(horizontal) && horizontal < -0)
        {
            heroDestPosition = hero.transform.position + Vector3.left;
            if (CanMove(goHero, heroDestPosition))
                StartCoroutine(LerpPosition(goHero, heroDestPosition, duration)); //will do the lerp over two seconds
        }
        if (heroIsMoving)
        {
            MobsTurn();
        }
        hero.setBubbleTextPosition(hero.transform.position + Vector3.up * 0.8f);
    }

    void MobsTurn()
    {
        var activeMobs = Mobs.Where(mob => distanceFromHero(mob.transform.position) <= 5);
        var dir = (rng.Next(10) % 2 == 0) ? 
         (rng.Next(10) % 2 == 0)? Vector3.left : Vector3.right :
         (rng.Next(10) % 2 == 0)? Vector3.up : Vector3.down;
        activeMobs.ToList().ForEach(mob =>
        {
            var newPosition = mob.transform.position + dir;
            if (CanMove(mob, newPosition))
                StartCoroutine(LerpPosition(mob, newPosition, duration));
        });
    }

    private bool CanMove(GameObject go, Vector3 dstPosition)
    {
        if (go.tag == "HERO")
        {
            hero.setBubbleTextColor(new Color(255, 255, 255, 255));
            hero.setBubbleTextMessage("MOVING");
            sound.clip = hero.getAudioClipFootStep();
        }
        else if (dstPosition == heroDestPosition)
        {
            return false;
        }

        //    Collider[] colliders = Physics.OverlapSphere(position, 0.0f);
        //    return colliders.Length == 0; //returns all the colliders that contain your position
        RaycastHit2D hit = Physics2D.Raycast(dstPosition, Vector2.zero, 15f, layerMask);
        if (hit.collider != null && hit.collider.tag == "WALL")
        {
            return false; // tHere's a wall
        }
        else if (hit.collider != null && hit.collider.tag == "COIN")
        {
            if (go.tag == "HERO")
            {
                hero.setBubbleTextColor(new Color(0, 255, 255, 255));
                hero.setBubbleTextMessage("I'M RICH!");
                sound.clip = hero.getAudioClipCoin();
                score += 1;
                hero.setScoreTextValue(score);
                Destroy(hit.collider.transform.gameObject);
            }
            else
            {
                return false;
            }
        }
        else if (hit.collider != null && hit.collider.tag == "MUSHROOM")
        {
            if (go.tag == "HERO")
            {
                hero.setBubbleTextColor(new Color(50, 255, 50, 0));
                hero.setBubbleTextMessage("I FEEL GOOD!");
                hero.heal(hit.collider.GetComponent<Mushroom>().HealingPoints);
                sound.clip = hero.getAudioClipYummy();
                Destroy(hit.collider.transform.gameObject);
            }
            else
            {
                return false;
            }
        }
        else if (hit.collider != null && hit.collider.tag == "MOB" && go.tag == "HERO")
        {
            Debug.Log("HIT");
            int damageDone = Combat.Attack(hero, hit.collider.GetComponent<Entity>());
            if (damageDone > 0)
            {
                // check mob HP
                Mob mob = hit.collider.GetComponent<Mob>();
                if (mob.applyDamage(damageDone) < 0) {
                    hero.addExpPoints(mob.ExpPoints);
                    Mobs.Remove(hit.collider.transform.gameObject);
                    Destroy(hit.collider.transform.gameObject);
                }
                Debug.Log($"Damage done => {damageDone}");
                hero.setBubbleTextColor(new Color(255, 255, 255, 255));
                hero.setBubbleTextMessage($"{damageDone}");
            }
            else
            {
                hero.setBubbleTextMessage("MISS!");
                Debug.Log($"Damage done => Miss!");
            }
            StartCoroutine(HitCoroutine(go, duration));
            // needs fixing bubbleText.transform.position = this.transform.position;
            return false;
        }
        else if (hit.collider != null && hit.collider.tag == "HERO")
        {
            int damageDone = Combat.Attack(go.GetComponent<Entity>(), hero);
            hero.applyDamage(damageDone); // TODO check if hero is dead
            hero.setBubbleTextColor(new Color(255, 0, 0, 255));
            hero.setBubbleTextMessage($"{damageDone}");
            return false;
        }
        return true; // no wall!
    }

    IEnumerator LerpPosition(GameObject go, Vector3 targetPosition, float duration)
    {
        isCoroutineBusy = true;
        if (go.tag == "HERO")
        {
            sound.Play();
        }
        float time = 0;
        Vector3 startPosition = go.transform.position;

        while (time < duration)
        {
            go.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        go.transform.position = targetPosition;
        isCoroutineBusy = false;
        if (go.tag == "HERO")
        {
            sound.Stop();
            hero.setBubbleTextColor(new Color(255, 255, 255, 255));
            hero.setBubbleTextMessage("IDLE");
        }
        //Input.ResetInputAxes();
    }


    IEnumerator HitCoroutine(GameObject go, float duration)
    {
        isCoroutineBusy = true;
        if (go.tag == "HERO")
        {
            //sound.Play();
        }
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            yield return null;
        }
        isCoroutineBusy = false;
        if (go.tag == "HERO")
        {
            // sound.Stop();
            hero.setBubbleTextColor(new Color(255, 255, 255, 255));
            hero.setBubbleTextMessage("IDLE");
        }
    }


    private float distanceFromHero(Vector2 mobPosition)
    {
        return Vector2.Distance(mobPosition, hero.transform.position);
    }
}
