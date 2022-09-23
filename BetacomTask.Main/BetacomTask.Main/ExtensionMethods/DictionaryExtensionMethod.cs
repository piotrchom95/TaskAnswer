namespace BetacomTask.Main.ExtensionMethods
{
    public static class DictionaryExtensionMethod
    {

        public static void AddRecordToDictionaryOrUpdateExistingOne<TKey>(this Dictionary<TKey, int> dictionary, TKey key , int value ) where TKey : notnull
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] += value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
               
        public static float GetMaxValueFromDictionary(this Dictionary<int, float> dictionary)
        {
            return dictionary.Max(r => r.Value);
        }

        public static int? GetUniqueKeyOfMaxValueFromDictionary(this Dictionary<int, float> resultOfMatchingByGpsId)
        {
            try
            {
               return resultOfMatchingByGpsId.SingleOrDefault(x => x.Value == resultOfMatchingByGpsId.Values.Max()).Key;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // powinno się obsłużyć to jakims loggerem
                return null;
            }
        }

    }
}
