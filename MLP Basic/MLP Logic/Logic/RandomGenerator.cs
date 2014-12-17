using System;

namespace MLP_Logic.Logic
{
    public static class RandomGenerator
    {
        static public Random Random = new Random((int)DateTime.Now.Ticks);
    }
}
