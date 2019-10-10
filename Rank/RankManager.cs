using UnityEngine;

public class RankManager : MonoBehaviour
{
    public Rank[] Ranks = new Rank[35];

    public int[] GetXpTable()
    {
        int[] _xpTable = new int[35];
        
        for(int i = 0; i < 35; i++)
        {
            _xpTable[i] = Ranks[i].XP;
            Debug.Log(Ranks[i].XP);
        }
        
        return _xpTable;
    }

    public Rank[] CalculateRank(int XP)
    {
        Rank[] _ranks = new Rank[2];
        int[] _xpTable = GetXpTable();


        for(int i = 0; i < _xpTable.GetLength(0); i++)
        {
            if((_xpTable[i] < XP) && (XP < _xpTable[i + 1]))
            {
                _ranks[0] = Ranks[i];
                _ranks[1] = Ranks[i + 1];
                break;
            }
            else if(XP < _xpTable[i])
            {
                _ranks[0] = Ranks[0];
                _ranks[1] = Ranks[1];
                break;
            }
        }

        return _ranks;
    }
}
