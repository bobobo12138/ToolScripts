using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueChangeListener
{
    public bool isChange { get; private set; } = false;

    Action _onValueChange;
    Action onValueChange
    {
        get
        {
            if (_onValueChange == null)
            {
                _onValueChange += () => {  };
            }
            return _onValueChange;
        }
        set
        {
            _onValueChange = value;
        }
    }

    Action _onSave;
    Action onSave
    {
        get
        {
            if (_onSave == null)
            {
                _onSave += () => { };
            }
            return _onSave;
        }
        set
        {
            _onSave = value;
        }
    }



    public void AddChangeListener(Action action)
    {
        onValueChange += action;
    }

    public void AddSaveListener(Action action)
    {
        onSave += action;
    }

    /// <summary>
    /// 已经对change进行了操作，重置
    /// </summary>
    public void SaveChange()
    {
        isChange = false;
        onSave();
    }

    /// <summary>
    /// 发生了变化，通知
    /// </summary>
    public void ValueChange()
    {
        isChange = true;
        onValueChange();
    }

}
