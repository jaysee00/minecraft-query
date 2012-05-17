using MinecraftQuery.Connections;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinecraftQuery
{
    class SettableCondition : Condition
    {
        public bool Value { get; set; }

        public override bool Evaluate()
        {
            return Value;
        }
    }
}
