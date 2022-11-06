using EntityCreater.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityCreater.AddDatabase
{
    public interface IEntityBuild
    {
        ICreateEntityFile EntityBuild(Action<EntityBuildModel> options);
    }
}