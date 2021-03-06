﻿using GhostGen;

public class BaseController : NotificationDispatcher
{
    protected UIView _view;

    protected ViewFactory viewFactory
    {
        get
        {
            return Singleton.instance.gui.viewFactory;
        }
    }

    protected UIView view
    {
        get { return _view; }
        set { _view = value; }
    }

    public virtual void Start()
    {
        
    }

    public virtual void RemoveView()
    {
        if(view != null)
        {
            viewFactory.RemoveView(view);
        }
    }
}
