using PetryNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.Services.LTL
{
    internal class UnaryOperatorNode : AstNode
    {
        public string Operator { get; private set; }
        public AstNode Operand { get; private set; }

        public UnaryOperatorNode(string op, AstNode operand)
        {
            Operator = op;
            Operand = operand;
        }
        public override bool Evaluate(List<TransitionModel> tokenHistory)
        {
            return Operator switch
            {
                "!" => !Operand.Evaluate(tokenHistory),
                "X" => EvaluateNext(tokenHistory),
                "F" => EvaluateFuture(tokenHistory),
                "G" => EvaluateGlobally(tokenHistory),
                _ => throw new NotSupportedException($"Unsupported operator: {Operator}")
            };
        }

        private bool EvaluateNext(List<TransitionModel> tokenHistory)
        {
            if (tokenHistory.Count < 2)
                return false;

            var nextHistory = tokenHistory.Skip(1).ToList();
            return Operand.Evaluate(nextHistory);
        }

        private bool EvaluateGlobally(List<TransitionModel> tokenHistory)
        {
            return tokenHistory.All(t => Operand.Evaluate(new List<TransitionModel> { t }));
        }

        private bool EvaluateFuture(List<TransitionModel> tokenHistory)
        {
            for (int i = 0; i < tokenHistory.Count; i++)
            {
                if (Operand.Evaluate(tokenHistory.Skip(i).ToList()))
                    return true;
            }
            return false;
        }
    }
}
