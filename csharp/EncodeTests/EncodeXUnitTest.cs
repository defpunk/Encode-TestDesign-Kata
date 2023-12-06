using System;
using EncodeKata;
using Xunit;

namespace EncodeTests;

public class EncodeXUnitTest
{
    [Fact]
    public void MessageTest()
    {
        var command = new SessionModificationCmd(1, 1);
        var data = new ByteBuffer();
        command.SetXyzTimer(XyzTimerUnit.MultiplesOfHours, 23);
        command.SetPqvl(1);
        command.Encode(data);
        var hex = new HexStringEncoder();
        var hexStr = hex.Encode(data);
        Console.Write("Hex: " + hexStr + "\n");
        Assert.Equal("03010101083791", hexStr);

        command.SetXyzTimer(XyzTimerUnit.MultipliesOfMinutes, 32); // outside range(31), expect 31
        command.Encode(data);
        hexStr = hex.Encode(data);
        Console.Write("Hex: " + hexStr + "\n");
        Assert.Equal("03010101085f91", hexStr);
        
        command.SetXyzTimer(XyzTimerUnit.TimerDeactivated, 2); // deactivated, expect value 0
        command.Encode(data);
        hexStr = hex.Encode(data);
        Console.Write("Hex: " + hexStr + "\n");
        Assert.Equal("03010101080091", hexStr);
    }

    public class HexStringEncoder
    {
        public string Encode(ByteBuffer buffer)
        {
            string str = "";
            while (buffer.GetAvailable() > 0)
            {
                int octet = buffer.Read();
                int tmp1 = (octet >> 4) & 0x0F;
                int tmp2 = octet & 0x0F;
                char char1;
                char char2;
                if (tmp1 <= 9)
                {
                    char1 = (char)(tmp1 + '0');
                }
                else
                {
                    char1 = (char)(tmp1 - 10 + 'a');
                }

                if (tmp2 <= 9)
                {
                    char2 = (char)(tmp2 + '0');
                }
                else
                {
                    char2 = (char)(tmp2 - 10 + 'a');
                }

                str += char1;
                str += char2;
            }

            return str;
        }
    }   
}