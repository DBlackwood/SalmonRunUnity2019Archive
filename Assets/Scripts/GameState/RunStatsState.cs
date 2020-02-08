using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//so far this method does nothing but declare stuff. Someday it will write a data file.
public class SalmonRunStats: Object
{
    public struct turnsDatStru
    {
        List<int> parentSmall;
        List<int> parentMedium;
        List<int> parentLarge;
        List<int> offspringSmall;
        List<int> offspringMedium;
        List<int> offspringLarge;
        List<int> offspringFemale;
        List<int> offspringMale;
        List<int> barSurvive;
        List<int> barFemale;
        List<int> barMale;
        List<int> barSmall;
        List<int> barMedium;
        List<int> barLarge;
        List<int> numAnglerPlain;
        List<int> numAnglerRanger;
        List<int> numDam;
        List<int> numLadder;
    }
    public struct verDataStru
    {
        string Version;           //DemoLevel>Canvas_Updated>StartPanel>Text2
        // Project: Assets>Prefabs>Towers>FishermanTower; Hierachy: FishermanTower>TokenBase (angler instead of fisherman)
        int anglerEffectRadius;
        int anglerTimePerApplyEffect;
        int anglerDefaultCatchRate;
        int anglerNumFlashesPerCatch;
        float anglerDefaultSmallCatchRate;
        float anglerDefaultMediumCatchRate;
        float anglerDefaultLargeCatchRate;
        // Project: Assets>Prefabs>Towers>RangerTower; Hierachy: RangerTower>TokenBase 
        int rangerEffectRadius;
        int rangerTimePerApplyEffect;
        float rangerSlowdownEffectSmall;
        float rangerSlowdownEffectMedium;
        float rangerSlowdownEffectLarge;
        int rangerRegulationSuccessRate;
        int rangerNumFlashesPerCatch;
        float damDefaultCrossingRate;    //Project; Assets>Prefabs>Filters>Dam
        //Project; Assets>Prefabs>Filters>DamLadder
        float ladderSmallDefaultCrossingRate;
        float ladderMediumDefaultCrossingRate;
        float ladderLargeDefaultCrossingRate;
        int schoolNumNestSights;    //not really in school look in EndOfLevel which is in three different spots
        // Project: Assets>_Scenes>MapV2>DemoLevel; Hierarchy: DemoLevel>School
        int schoolInitialNumFish;
        int schoolMinOffspring;
        int schoolMaxOffspring;
        float schoolTimeBetweenWaves;
        int schoolFishPerWave;
    }

    public verDataStru versionData;
    public turnsDatStru turnData;
}

/**
 * Game state where statistics are displayed (presumably following the Run state)
 */
public class RunStatsState : GameState
{
    /**
     * Handle entry into the RunStats state
     */
    public override void Enter(GameState oldState)
    {
        // invoke event
        GameEvents.onStartRunStats.Invoke();
    }

    /**
     * Handle exiting the RunStats state
     */
    public override void ExitState()
    {

    }

    /**
     * Handle any updating actions that need to happen mid-state
     */
    public override void UpdateState()
    {

    }
}
