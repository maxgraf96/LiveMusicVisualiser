/**
 * This class continuously reads incoming values and keeps track of their maximums for auto mapping/scaling the ranges
 */
public class AutoMapper
{
    // FFT frequency band 0
    public static float F0 = 0f;
    // FFT frequency band 2
    public static float F2 = 0f;
    
    // Update frequency band 0 if the incoming value is higher
    public static void updateF0(float f0)
    {
        if(f0 > F0)
        {
            F0 = f0;
        }
    }

    // Update frequency band 2 if the incoming value is higher
    public static void updateF2(float f2)
    {
        if (f2 > F2)
        {
            F2 = f2;
        }
    }
}
