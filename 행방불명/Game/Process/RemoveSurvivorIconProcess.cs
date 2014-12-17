using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Game.Process
{
    public class RemoveSurvivorIconProcess
        : EmptyProcess
    {
        GameStage stage;
        string id;

        public RemoveSurvivorIconProcess(
            GameStage stage,
            string id
            )
        {
            this.stage = stage;
            this.id = id;
        }

        public override void End()
        {
            Console.WriteLine("{0} 생존자 찾았는데, 아이콘 제거 중.", id);
            foreach (var surv in stage.Map.Survivors)
            {
                if (surv.Id.Equals(id))
                {
                    Console.WriteLine("제거됨.");
                    surv.IsFound = false;
                    break;
                }
            }
        }
    }
}
