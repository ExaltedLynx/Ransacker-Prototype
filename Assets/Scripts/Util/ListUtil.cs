using System.Collections.Generic;

public class ListUtil
{
    public static List<T> GetKMostFrequent<T>(List<T> elements, int k)
    {
        //Dict to store each element count
        Dictionary<T, int> elementCount = new Dictionary<T, int>();
        foreach(T element in elements)
        {
            if (elementCount.ContainsKey(element))
                elementCount[element]++;
            else
                elementCount.Add(element, 1);
        }

        //2d array to store elements according to their frequency
        List<KeyValuePair<T, int>> freqList = new List<KeyValuePair<T, int>>();
        foreach(KeyValuePair<T, int> kv in elementCount)
            freqList.Add(kv);

        //sorts frequencyList in descending order
        freqList.Sort((kv2, kv1) => {
            if (kv1.Value == kv2.Value)
                return 0;

            return kv1.Value.CompareTo(kv2.Value);
        });

        List<T> list = new List<T>();
        for (int i = 0; i < freqList.Count; i++)
        {
            list.Add(freqList[i].Key);
            if (i == k - 1)
                break;
        }
            

        return list;
    }
}
