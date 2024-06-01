using Sqids;

namespace OQS.CoreWebAPI.Shared;

public class LiveQuizCode
{
    private static SqidsEncoder<int> encoder = new SqidsEncoder<int>(new()
    {
        Alphabet = "8KG3h0mZCBQSEIeYrlwaUbHjqAWOg1dcRL4i5ousPvnk9tzX7pFT6MyJfNx2DV",
        MinLength = 6
    });
    
    private static Random random = new Random();
    public static String Generate()
    {
        int num = random.Next(0,10000);
      
        return encoder.Encode(num);
    }
}