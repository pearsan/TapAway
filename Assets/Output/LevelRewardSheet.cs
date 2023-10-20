using System.Collections.Generic;
using MagicExcel;

[System.Serializable]
public class LevelRewardSheet
{
    /// <summary>
    /// comment
    /// </summary>
    public int Level;

    /// <summary>
    /// comment
    /// </summary>
    public string Reward;


    private static Dictionary<int, LevelRewardSheet> dictionary = new Dictionary<int, LevelRewardSheet>();

    /// <summary>
    /// Get LevelRewardSheet by Level
    /// </summary>
    /// <param name="Level"></param>
    /// <returns>LevelRewardSheet</returns>
    public static LevelRewardSheet Get(int Level)
    {
        return dictionary[Level];
    }
    
    public static Dictionary<int, LevelRewardSheet> GetDictionary()
    {
        return dictionary;
    }

    public static void SetDictionary(Dictionary<int, LevelRewardSheet> dic) {
        dictionary = dic;
    }
}
