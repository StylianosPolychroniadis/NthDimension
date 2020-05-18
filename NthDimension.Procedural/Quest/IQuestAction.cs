using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest
{
    public abstract class IQuestAction
    {

        abstract public void DisplaySingleAction(int indent);
        abstract public QuestObject GetNPC();
    }
}
