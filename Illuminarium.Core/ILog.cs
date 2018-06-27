using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core
{
    public interface ILog
    {
        void Message( string message );

        void Message( string message, int level );
        void Error( Exception ex );
    }
}
