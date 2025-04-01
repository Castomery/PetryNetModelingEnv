using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.Models
{
    public class TransitionModel : NodeModel
    {
        public string Name { get; set; } = "T";
        public string LTLExpression { get; set; } = "";

        public TransitionModel(int id, string name) : base(id)
        {
            Name = name;
        }

        public void addLTLExpression(string expression)
        {
            LTLExpression = expression;
        }

        public bool CanFire(List<TransitionModel> history)
        {
            if (string.IsNullOrEmpty(LTLExpression))
                return true; // No LTL expression means the transition is always enabled

            // Implement LTL evaluation logic here
            // For now, assume the expression is a simple boolean condition
            return EvaluateLtlExpression(history);
        }

        private bool EvaluateLtlExpression(List<TransitionModel> history)
        {
            // Implement LTL evaluation logic here
            throw new NotImplementedException();
        }
    }
}
