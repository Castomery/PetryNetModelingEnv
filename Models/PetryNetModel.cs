using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.Models
{
    public class PetryNetModel
    {
        public ObservableCollection<PlaceModel> Places { get; } = new();
        public ObservableCollection<TransitionModel> Transitions { get; } = new();
        public ObservableCollection<ArcModel> Arcs { get; } = new();

        public ObservableCollection<TokenModel> AllTokens { get; } = new();

        public PlaceModel CreatAndGetPlace(int placeId, string Name)
        {
            PlaceModel place = new PlaceModel(placeId, Name);
            Places.Add(place);
            return place;
        }

        public TransitionModel CreateAndGetTransition(int transitionId, string Name)
        {
            TransitionModel transition = new TransitionModel(transitionId, Name);
            Transitions.Add(transition);
            return transition;
        }

        public ArcModel CreateAndGetArc(int arcId, NodeModel source, NodeModel target)
        {
            ArcModel arc = new ArcModel(arcId, source, target);
            Arcs.Add(arc);
            return arc;
        }

        public void AddPlace(PlaceModel place)
        {
            if (place == null)
                throw new ArgumentNullException(nameof(place));

            Places.Add(place);
        }

        public void AddTransition(TransitionModel transition)
        {
            if (transition == null)
                throw new ArgumentNullException(nameof(transition));

            Transitions.Add(transition);
        }

        public void AddArc(ArcModel arc)
        {
            if (arc == null)
                throw new ArgumentNullException(nameof(arc));

            if (!Places.Contains(arc.Source as PlaceModel) && !Transitions.Contains(arc.Source as TransitionModel))
                throw new InvalidOperationException("Source element not found in the Petri Net.");

            if (!Places.Contains(arc.Target as PlaceModel) && !Transitions.Contains(arc.Target as TransitionModel))
                throw new InvalidOperationException("Target element not found in the Petri Net.");

            Arcs.Add(arc);
        }

        public void RemovePlace(PlaceModel place)
        {
            if (place == null)
                throw new ArgumentNullException(nameof(place));

            RemoveAllConnectedArcs(place);
            Places.Remove(place);
        }

        public void RemoveAllConnectedArcs(NodeModel node)
        {
            if (node == null) throw new ArgumentNullException();

            for (int i = 0; i < Arcs.Count; i++)
            {
                if (Arcs[i].Source == node || Arcs[i].Target == node)
                {
                    Arcs.RemoveAt(i);
                }
            }
        }

        public void RemoveArc(ArcModel arc)
        {
            if (arc == null)
                throw new ArgumentNullException(nameof(arc));

            Arcs.Remove(arc);
        }

        public void RemoveTransition(TransitionModel transition)
        {
            if (transition == null)
                throw new ArgumentNullException(nameof(transition));

            // Remove all arcs connected to this transition
            RemoveAllConnectedArcs(transition);
            Transitions.Remove(transition);
        }

        public bool IsTransitionEnabled(TransitionModel transition)
        {
            bool isEnabled = false;
            if (transition == null)
                throw new ArgumentNullException(nameof(transition));

            List<ArcModel> linkedArcs = Arcs.Where(arc => arc.Target == transition).ToList();

            foreach (var arc in linkedArcs)
            {
                if (arc.Source != null)
                {

                    PlaceModel place = arc.Source as PlaceModel;

                    if (place.Tokens.Count < arc.Weight)
                    {
                        isEnabled = false;
                        return isEnabled;
                    }
                    else
                    {
                        isEnabled = true;
                    }
                }
            }
            return isEnabled;
        }

        public void FireTransition(TransitionModel transition)
        {
            if (transition == null)
                throw new ArgumentNullException(nameof(transition));

            if (!IsTransitionEnabled(transition))
                throw new InvalidOperationException("Transition is not enabled.");

            // Get the token history from the source place
            var sourcePlace = Arcs
                .Where(arc => arc.Target == transition && arc.Source is PlaceModel)
                .Select(arc => arc.Source as PlaceModel)
                .FirstOrDefault();

            if (sourcePlace == null)
                throw new InvalidOperationException("No source place found for the transition.");

            //var tokenHistory = sourcePlace.Tokens
            //    .SelectMany(token => token.History)
            //    .ToList();

            // Evaluate the LTL expression
            if (!transition.CanFire(new List<TransitionModel>()))
                throw new InvalidOperationException("LTL expression prevents firing.");

            // Move tokens based on the arcs
            foreach (var arc in Arcs)
            {
                if (arc.Source == transition && arc.Target is PlaceModel targetPlace)
                {
                    // Produce tokens in the target place
                    for (int i = 0; i < arc.Weight; i++)
                    {
                        var token = new TokenModel();
                        //token.AddTransitionToHistory(transition, tokenHistory);
                        targetPlace.AddToken(token);
                    }
                }
                else if (arc.Target == transition && arc.Source is PlaceModel source)
                {
                    // Consume tokens from the source place
                    if (source.Tokens.Count < arc.Weight)
                        throw new InvalidOperationException("Not enough tokens to fire transition.");

                    for (int i = 0; i < arc.Weight; i++)
                    {
                        var token = source.Tokens.Last(); // Remove the last token
                        source.RemoveToken(token);
                    }
                }
            }
        }


    }
}
