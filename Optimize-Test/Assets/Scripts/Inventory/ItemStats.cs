using System;

[Serializable]
public class ItemStats
{
    public int Fur;
    public int Fang;
    public int Eyes;

    public int Sum => Fur + Fang + Eyes;
}