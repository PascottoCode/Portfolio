using System.Collections.Generic;
using UnityEngine;

public class FatigueList<T>
{
    public FatigueList(float duration)
    {
        _fatigueList = new Dictionary<T, float>();
        _duration = duration;
    }
    
    private Dictionary<T, float> _fatigueList;
    private float _duration;

    public bool CheckFatigue(T other)
    {
        if (_fatigueList.ContainsKey(other))
        {
            return Time.time - _fatigueList[other] < _duration;
        }
        return false;
    }
    public bool UpdateCheckFatigue(T other)
    {
        if (_fatigueList.ContainsKey(other))
        {
            if (Time.time - _fatigueList[other] < _duration)
            {
                return true;
            }
            _fatigueList[other] = Time.time;
        }
        else
        {
        	_fatigueList.Add(other, Time.time);
        }
        
	    return false;
    }
    public void Clear()
    {
        _fatigueList.Clear();
    }
}
