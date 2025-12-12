using System;
using System.Collections.Generic;

namespace Corno.Web.Models.Base;

[Serializable]
public class CornoModel : ICornoModel
{
    public virtual void Reset()
    {
    }

    public virtual IList<CornoModel> GetDetails()
    {
        return null;
    }

    public virtual bool UpdateDetails(CornoModel newModel)
    {
        return false;
    }

    public virtual void Copy(CornoModel other)
    {

    }
}