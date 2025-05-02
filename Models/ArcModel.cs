using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.Models
{
    public class ArcModel : NodeModel
    {
        public int Id { get; private set; }
        public NodeModel Source { get; private set; }
        public NodeModel Target { get; private set; }
        public int Weight { get; private set; } = 1;

        public ArcModel(int id, NodeModel source, NodeModel target) : base(id)
        {
            Source = source;
            Target = target;
        }

        public void ChangeWeight(int weight)
        {
            Weight = weight;
        }

        public void IncreaseWeightByOne()
        {
            Weight++;
        }

        private bool IsPlaceToTransition() => Source is PlaceModel && Target is TransitionModel;
        private bool IsTransitionToPlace() => Source is TransitionModel && Target is PlaceModel;

        public bool CanBeLinked()
        {
            return IsPlaceToTransition() || IsTransitionToPlace();
        }

        public void ConnectArc(NodeModel source, NodeModel target)
        {
            if(CanBeLinked())
            {
                Source = source;
                Target = target;
            }
        }
    }
}
