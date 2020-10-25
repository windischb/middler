using System;
using System.Collections.Generic;
using System.Text;
using middler.Common.SharedModels.Models;

namespace middler.Common
{
    public interface IMiddlerContext
    {
        IMiddlerRequestContext Request { get; }

        IMiddlerResponseContext Response { get; }

        SimpleDictionary<object> PropertyBag { get; }

        IServiceProvider RequestServices { get; }
        //void ForwardBody();
    }
}
