using System;
using DotrModdingTool2IMGUI;

namespace GameplayPatches {
  public class Patch
  {
    protected DataAccess dataAccess {
      get { 
        return DataAccess.Instance;
      }
    }


    public void ApplyOrRemove(bool apply)
    {
      if (apply)
      {
        Apply();
      }
      else
      {
        Remove();
      }
    }

    public virtual bool IsApplied()
    {
      throw new NotImplementedException();
    }

    protected virtual void Apply()
    {
      throw new NotImplementedException();
    }

    protected virtual void Remove()
    {
      throw new NotImplementedException();
    }
  }
}
