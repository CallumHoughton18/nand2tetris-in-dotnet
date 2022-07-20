using System;
					
public class Program
{
    public static void Main()
    {
        //var result = divide(16, 8);
        //Console.WriteLine(result);
		
        Console.WriteLine(sqrroot(8));
    }
	
    public static int divide(int x, int y)
    {
        if (y > x) return 0;
        int q = divide(x, y + y);
        if((x - 2 * q * y) < y) {
            var result = 2 * q;
            return result;
        }
        else 
        {
            return 2 * q + 1;
        }
    }
	
    public static int sqrroot(int x)
    {
		
        // x = 8
        // low = 0, high = 8
        // med = 4
        // medsqred = 16
        // 16 > 8 so high = 4 - 1 = 3
		
        // low = 0, high = 3
        // med = 1
        // medsqred = 1
        // 1 < 8 so low = 2
		
        // low = 2, high = 3
        // med = 2
        // medsqred = 4 
        // 4 < 8 so low = 3
		
        // low = 3, high = 3 so therefore is 2
		
        int low = 0;
        int high = x;	
		
        while (low < (high + 1))
        {
            int med = (low + high) / 2;
            int medSqred = med * med;
			
            if (medSqred < (x + 1)) 
            {
                low = med + 1;
            }
            else 
            {
                high = med - 1;
            }
        }
		
        return low - 1;	
    }
}