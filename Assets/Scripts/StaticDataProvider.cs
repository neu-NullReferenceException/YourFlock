using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticDataProvider
{
    public static int material = 0;     // az er�forr�s
    public static int radicality = 0; // a radikalit�st m�ri, ha negat�v akkor az "�nzetlen ir�nyba" tartunk
    public static int ammo; // l�szeradagokat m�r: 1 k�ldet�s 1 f�nek 1 adag l�szer HA visz mag�val l�fegyvert 
    public static int food = 12000;   //(kcal) az egyszer�s�g kedv��rt minden ember hogy ne �hezzen default 1000 kcal/ nap fogyaszt
    public static int defaultFoodConsumption = 1000;
    public static int defaultDeathMentalChange = 5;
    public static List<InventoryItem> inventoryItems = new List<InventoryItem>();
    public static List<Follower> followers = new List<Follower>();
    public static List<Follower> banishedPeople = new List<Follower>();
    public static List<Follower> strikeTeam = new List<Follower>();
    public static bool[] HostilityMatrix = { // az index reprezentent�lja a teamID t (r�szletek az IDamagable interface le�r�s�ban (sz�nd�kosan van el�rva)) 
        false,
        false,
        true,
        false,
        true,
    };

    public static string[] firstNames = {
    "Olivia","Zoe","Ava","Sophia","Emma","Mia","Isabella","Ella","Grace","Harper","Abigail","Lily","Chloe","Madison","Anne",
    "Ethan","Jackson","Joe","Mason","Aiden","Liam","Noah","Lucas","Logan","Owen","Carter","Samuel","Benjamin","Gabriel","Daniel"
    };
    public static string[] lastNames ={
    "Williams","Harrison","Gonzalez","Mitchell","Scott","Fisher","Palmer","Jenkins","Ward","Harrison","Evans","Fleming","Bishop","Wong",
    "Barnes","Hansen","Gill","Lawson","Fitzgerald","Horton","Montgomery","Walters","Brewer","Barnett","Reeves","Wheeler","Riley","Hudson",
    "Baxter"
    };
    public static Sprite[] profilePics;

    public static void AddRadicallity(int amount)
    {
        radicality += amount;
    }

    public static void AddFood(int amount)
    {
        food += amount;
    } 

    public static int CalculateFoodConsumption()
    {
        int consumption = 0;
        foreach (Follower item in followers)
        {
            consumption += item.dayliRation;
        }
        return consumption;
    }

    public static void FeedPopulation()
    {
        foreach (Follower item in followers)
        {
            if(food > item.dayliRation)
            {
                item.food += item.dayliRation;
                item.food -= defaultFoodConsumption;
                food -= item.dayliRation;
            }
            else
            {
                item.food += food;
                item.food -= defaultFoodConsumption;
                food = 0;
            }
        }
    }

    public static void OneTimeCollectiveMentalChange(int amount)
    {
        foreach (Follower item in followers)
        {
            item.mentalState -= amount;
        }
    }

    public static Follower GetWeakestFollower()
    {
        int hlow = int.MaxValue;
        Follower weakest = null;
        foreach (Follower f in followers)
        {
            if(f.health < hlow)
            {
                weakest = f;
                hlow = weakest.health;
            }
        }

        return weakest;
    }

    public static Follower GetYungestFollower()
    {
        Follower yungest = followers[0];
        foreach (Follower f in followers)
        {
            if (f.age < yungest.age)
            {
                yungest = f;
            }
        }
        return yungest;
    }

    public static void DieInConbat(Follower f)
    {
        followers.Remove(f);
        OneTimeCollectiveMentalChange(defaultDeathMentalChange);
    }

    public static void AddRandomFollower(int number)
    {
        for (int i = 0; i < number; i++)
        {
            string name = firstNames[Random.Range(0, firstNames.Length)] + " " + lastNames[Random.Range(0, lastNames.Length)];
            Follower f = new Follower(name, Random.Range(5, 65));
            //f.profilePic = profilePics[Random.Range(0, profilePics.Length)];
            followers.Add(f);
        }
    }
}
