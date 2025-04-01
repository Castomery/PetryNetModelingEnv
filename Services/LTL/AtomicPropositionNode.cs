using PetryNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.Services.LTL
{
    internal class AtomicPropositionNode : AstNode
    {
        public string Proposition {  get; private set; }

        public AtomicPropositionNode(string proposition)
        {
            Proposition = proposition;
        }
        public override bool Evaluate(List<TransitionModel> tokenHistory)
        {
            return tokenHistory.Any(t => t.Name == Proposition);
        }
    }
}
