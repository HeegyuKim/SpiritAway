using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Game.Process
{
    public class SoundPlayProcess
        : EmptyProcess
    {
        Program app;
        string sfx;

        public SoundPlayProcess(Program app, string sfx)
        {
            this.app = app;
            this.sfx = sfx;
        }


        public override void End()
        {
            app.Play2D(sfx);
        }
    }
}
