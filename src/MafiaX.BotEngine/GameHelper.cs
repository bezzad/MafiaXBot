using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaX.BotEngine
{
    public static class GameHelper
    {
        public static async Task<Dictionary<long, Player>> RandomCross(this Dictionary<long, Player> list)
        {
            var result = new Dictionary<long, Player>();
            var keys = list.Keys.ToArray();

            for (var i = 0; i < list.Count; i++)
            {
                if (i == list.Count - i)
                {
                    result.Add(keys[i], list[keys[i]]);
                    break;
                }

                result.Add(keys[i], list[keys[i]]);
                result.Add(keys[list.Count - i], list[keys[list.Count - i]]);
            }

            var rand = new Random();

            for (var i = 0; i < list.Count / 3; i++)
            {
                await Task.Delay(1);
                var fIndex = rand.Next(0, list.Count - 1);
                var sIndex = rand.Next(0, list.Count - 1);
                list.SwapPlayer(fIndex, sIndex);
            }

            return result;
        }

        public static Dictionary<long, Player> SwapPlayer(this Dictionary<long, Player> list, int firstItemIndex, int secondItemIndex)
        {
            var keys = list.Keys.ToArray();
            var firstKey = keys[firstItemIndex];
            var firstValue = list[firstKey];
            var secondKey = keys[secondItemIndex];

            list[firstKey] = list[secondKey];
            list[secondKey] = firstValue;

            return list;
        }
    }
}
