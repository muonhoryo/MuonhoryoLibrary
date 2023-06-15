using MuonhoryoLibrary.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuonhoryoLibrary
{
    /// <summary>
    /// Default value changed by add's and multiply's modifiers
    /// </summary>
    public sealed class CompositeParameter
    {
        public interface IConstModifier
        {
            public void RemoveModifier();
            public float Modifier { get; }
        }
        private sealed class ModifierHandler : IConstModifier
        {
            public ModifierHandler(float Modifier, SingleLinkedList<ModifierHandler> list)
            {
                RemoveHandlerAction = () => list.Remove(this);
                this.Modifier = Modifier;
            }
            public readonly float Modifier;
            private readonly Action RemoveHandlerAction;
            float IConstModifier.Modifier => Modifier;
            public void RemoveModifier() => RemoveHandlerAction();
        }
        private CompositeParameter() { }
        public CompositeParameter(float DefaultValue)
        {
            this.DefaultValue = DefaultValue;
            RecalculateSpeed();
        }
        public readonly float DefaultValue;
        public float CurrentValue { get; private set; }
        public event Action<IConstModifier> AddingAddModifierEvent;
        public event Action<IConstModifier> AddingMultiplyModifierEvent;
        public event Action<float> ValueHasBeenRecalculatedEvent;
        private readonly SingleLinkedList<ModifierHandler> AddersList = new SingleLinkedList<ModifierHandler>() { };
        private readonly SingleLinkedList<ModifierHandler> MultipliesList = new SingleLinkedList<ModifierHandler>() { };
        private void RecalculateSpeed()
        {
            CurrentValue = DefaultValue;
            foreach (var item in AddersList)
            {
                CurrentValue += item.Modifier;
            }
            foreach (var item in MultipliesList)
            {
                CurrentValue *= item.Modifier;
            }
            ValueHasBeenRecalculatedEvent?.Invoke(CurrentValue);
        }
        private IConstModifier AddModifier
            (float modifierValue, SingleLinkedList<ModifierHandler> list,
            Action<IConstModifier> runningEventAction)
        {
            ModifierHandler modifier = new ModifierHandler(modifierValue, AddersList);
            list.AddLast(modifier);
            runningEventAction(modifier);
            RecalculateSpeed();
            return modifier;
        }
        public IConstModifier AddModifier_Add(float speed)
        {
            return AddModifier(speed, AddersList, (item) => AddingAddModifierEvent?.Invoke(item));
        }
        public IConstModifier AddModifier_Multiply(float speed)
        {
            return AddModifier(speed, MultipliesList, (item) => AddingMultiplyModifierEvent?.Invoke(item));
        }
        public static explicit operator float(CompositeParameter i) => i.CurrentValue;
    }
}
