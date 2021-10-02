using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace threading_test
{
    class KillObject
    {
        bool _killCommand = false;
        public bool Get_KillCommand() 
        {
            return this._killCommand;
        
        }
        public void Set_KillCommand(bool setme)
        {
            _killCommand = setme;
        }
    }
}
