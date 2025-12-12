using System;

namespace Corno.Web.Services.Interfaces;

public interface IAssemblyService : IService
{
    #region -- Methods --

    Type GetType(string typeName);

    #endregion
}