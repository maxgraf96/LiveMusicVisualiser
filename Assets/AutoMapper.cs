/**
 * This class continuously reads incoming values and keeps track of their maximums for auto mapping the ranges
 */
public class AutoMapper
{
    public static float F0 = 0f;
    public static float F2 = 0f;
    public static void updateF0(float f0)
    {
        if(f0 > F0)
        {
            F0 = f0;
        }
    }

    public static void updateF2(float f2)
    {
        if (f2 > F2)
        {
            F2 = f2;
        }
    }
}
