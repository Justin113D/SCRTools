using System.Collections.Generic;

namespace SCR.Tools.Common
{
    public static class GenericHelper
    {
        public static string FindNextFreeKey<T>(this IDictionary<string, T> dict, string key, bool dictContainsOnlyLower = false)
        {
            // if name is whitespace or doesnt exist, then we can just return it
            string realKey = dictContainsOnlyLower ? key.ToLower() : key;
            if (key.Length == 0 || !dict.ContainsKey(realKey))
            {
                return key;
            }

            // check for a .### (e.g. .001) ending
            bool endsWithNumber = false;
            bool hasNumber = false;
            int index = key.Length - 1;
            for (; index > 0; index--)
            {
                char curChar = key[index];
                if (curChar == '.')
                {
                    if (hasNumber)
                        endsWithNumber = true;
                    break;
                }

                if (!char.IsNumber(curChar))
                    break;

                hasNumber = true;
            }

            int nameIndex = 1;
            string baseName = key;

            // if the name ends with a number, then we want to continue off of it
            if (endsWithNumber)
            {
                baseName = key[..index];
                nameIndex = int.Parse(key[++index..]);
            }

            // check names until we find a fitting one
            string newName = $"{baseName}.{nameIndex:D3}";
            while (dictContainsOnlyLower 
                ? dict.ContainsKey(newName.ToLower()) 
                : dict.ContainsKey(newName))
            {
                nameIndex++;
                newName = $"{baseName}.{nameIndex:D3}";
            }

            return newName;
        }
    }
}
