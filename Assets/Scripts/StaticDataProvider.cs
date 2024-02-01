using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticDataProvider
{
    public static int material = 10;     // az erõforrás
    public static int radicality = 0; // a radikalitást méri, ha negatív akkor az "önzetlen irányba" tartunk
    public static int ammo; // lõszeradagokat mér: 1 küldetés 1 fõnek 1 adag lõszer HA visz magával lõfegyvert 
    public static int food = 12000;   //(kcal) az egyszerûség kedvéért minden ember hogy ne éhezzen default 1000 kcal/ nap fogyaszt
    public static int defaultFoodConsumption = 1000;
    public static int defaultDeathMentalChange = 5;
    public static int defaultDaylyRadicalityChange = 5;
    public static int daysPassed = 1;
    public static List<InventoryItem> inventoryItems = new List<InventoryItem>();
    public static List<InventoryItem> craftingQueue = new List<InventoryItem>();
    public static List<Follower> followers = new List<Follower>();
    public static List<Follower> banishedPeople = new List<Follower>();
    public static List<Follower> strikeTeam = new List<Follower>();
    public static bool[] HostilityMatrix = { // az index reprezententálja a teamID t (részletek az IDamagable interface leírásában (szándékosan van elírva)) 
        false,
        false,
        true,
        false,
        true,
    };

    public static string[] firstNamesFemale = {
    "Olivia","Zoe","Ava","Sophia","Emma","Mia","Isabella","Ella","Grace","Harper","Abigail","Lily","Chloe","Madison","Anne", };
    public static string[] firstNamesMale = {
    "Ethan","Jackson","Joe","Mason","Aiden","Liam","Noah","Lucas","Logan","Owen","Carter","Samuel","Benjamin","Gabriel","Daniel"
    };
    public static string[] lastNames ={
    "Williams","Harrison","Gonzalez","Mitchell","Scott","Fisher","Palmer","Jenkins","Ward","Harrison","Evans","Fleming","Bishop","Wong",
    "Barnes","Hansen","Gill","Lawson","Fitzgerald","Horton","Montgomery","Walters","Brewer","Barnett","Reeves","Wheeler","Riley","Hudson",
    "Baxter"
    };
    public static Sprite[] profilePicsFemale;
    public static Sprite[] profilePicsMale;
    public static bool isFirstTime = true;

    public static List<Measure> passedLaws = new List<Measure>();
    public static bool lastStandAbilityUnlocked;
    public static bool emergencyRationsUnlocked;
    public static bool isLawAwailable = true; // 1 per nap
    public static InventoryItem foodRecipe;

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

    public static List<Follower> CheckStarveToDeath()
    {
        List<Follower> fs = new List<Follower>();
        /*foreach(Follower f in followers)
        {
            if(f.food <= 0)
            {
                fs.Add(f);
                DieInConbat(f);
            }
        }*/
        for (int i = 0; i < followers.Count; i++)
        {
            if (followers[i].food <= 0)
            {
                fs.Add(followers[i]);
                DieInConbat(followers[i]);
            }
        }
        return fs;
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
            string name;
            int gender = Random.Range(0, 2); //isMale = 1
            if(gender == 0)
            {
                name = firstNamesFemale[Random.Range(0, firstNamesFemale.Length)] + " " + lastNames[Random.Range(0, lastNames.Length)];
            }
            else
            {
                name = firstNamesMale[Random.Range(0, firstNamesMale.Length)] + " " + lastNames[Random.Range(0, lastNames.Length)];
            }
            Follower f = new Follower(name, Random.Range(5, 65));
            if(gender == 0)
            {
                f.profilePic = profilePicsFemale[Random.Range(0, profilePicsFemale.Length)];
            }
            else
            {
                f.profilePic = profilePicsMale[Random.Range(0, profilePicsMale.Length)];
            }
            //f.profilePic = profilePics[Random.Range(0, profilePics.Length)];
            followers.Add(f);
        }
    }

    public static int CountInventoryItem(InventoryItem item)
    {
        int c = 0;
        foreach (InventoryItem i in inventoryItems)
        {
            if(i.name == item.name)
            {
                c++;
            }
        }
        return c;
    }

    public static bool MeasureMeetsRequirement(Measure measure)
    {
        if (passedLaws.Contains(measure.conflictsWith))
        {
            return false;
        }
        if(measure.requiredToUnlock != null)
        {
            if (!passedLaws.Contains(measure.requiredToUnlock))
            {
                return false;
            }
        }

        return true;
    }
}
