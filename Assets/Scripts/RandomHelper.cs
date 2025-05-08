using System;

public static class RandomHelper
{
    private static Random random = new Random();

    public static float GetRandomFloat(float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
    }
}