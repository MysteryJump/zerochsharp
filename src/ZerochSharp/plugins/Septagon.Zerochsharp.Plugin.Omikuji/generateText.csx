using System;

public static string GenerateRandomedOmikujiText()
{
    var rand = new Random();

    var num = rand.Next(0, 100);
    if (num == 0)
    {
        return "大吉";
    }
    else if (num < 20)
    {
        return "吉";
    }
    else if (num < 45)
    {
        return "小吉";
    }
    else if (num < 55)
    {
        return "末吉";
    }
    else if (num < 99)
    {
        return "凶";
    }
    else
    {
        return "大凶";
    }
}