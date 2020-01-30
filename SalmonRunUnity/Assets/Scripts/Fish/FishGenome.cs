using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Struct representing a single gene pair in a genome
 */
public struct FishGenePair
{
    public string momGene;
    public string dadGene;
}

/**
 * Represents an individual fish's genome
 */
public class FishGenome
{
    public const string X = "X";
    public const string Y = "Y";
    public const string B = "B";
    public const string b = "b";

    // enum containing all possible types of genes within the genome
    public enum GeneType
    {
        Sex = 0,
        Size = 1
    }

    // property for length of the genome
    public static int Length
    {
        get
        {
            return System.Enum.GetNames(typeof(GeneType)).Length;
        }
    }

    // list of gene pairs
    private FishGenePair[] genePairList = new FishGenePair[Length];

    /**
     * Constructor for situations when genome is provided directly
     * 
     * @param genePairList List<FishGenePair> the list of gene pairs that makes up the genome
     */
    public FishGenome(FishGenePair[] genePairs)
    {
        // must make sure that the passed in list has a proper length of genome
        if (genePairs.Length == Length)
        {
            this.genePairList = genePairs;
        }
        else
        {
            throw new System.ArgumentException("Trying to create a gene pair list with the wrong number of genes!");
        }
        
    }

    /**
     * Constructor for situations where we want to create a genome from two parent genomes
     * 
     * @param momGenome FishGenePair The genome from the mother
     * @param dadGenome FishGenePair the genome from the father
     */
    public FishGenome(FishGenome momGenome, FishGenome dadGenome)
    {
        for (int index = 0; index < Length; index++)
        {
            FishGenePair newPair = new FishGenePair();
            newPair.momGene = Random.Range(0, 2) == 1 ? momGenome[index].momGene : momGenome[index].dadGene;
            newPair.dadGene = Random.Range(0, 2) == 1 ? dadGenome[index].momGene : dadGenome[index].dadGene;
            genePairList[index] = newPair;
        }
    }

    /**
     * Indexer using integer values
     */
    public FishGenePair this[int index]
    {
        get
        {
            return genePairList[index];
        }
    }

    /**
     * Indexer directly using the GeneType enum
     */
    public FishGenePair this[GeneType geneType]
    {
        get
        {
            return this[(int)geneType];
        }
    }

    /**
     * Determine whether this genome is male or female
     * 
     * @return bool True if the fish is male, false otherwise
     */
    public bool IsMale()
    {
        return this[GeneType.Sex].dadGene == Y;
    }
}
