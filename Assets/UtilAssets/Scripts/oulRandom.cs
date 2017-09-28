using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class oulRandom
{
    public static int[] GetRandomArray(int min, int max)
    {
        int[] randomArray = new int[max - min];
        for (int i = 0; i < max - min; i++)
        {
            randomArray[i] = i - min;
        }

        for (int i = 0; i < max - min; i++)
        {
            int r = Random.Range(min, max);
            int temp = randomArray[i];

            randomArray[i] = randomArray[r];

            randomArray[r] = temp;
        }
        return randomArray;
    }
}
