using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuonhoryoLibrary.Collections;

namespace MuonhoryoLibrary
{
    [Serializable]
    public sealed class CompositeInteger:CompositeParameter<int>
    {
        public CompositeInteger(int defaultValue):base(defaultValue) { }

        public event Action<IConstModifier<int>> AddingAddModifierEvent;
        public event Action<IConstModifier<int>> AddingMultiplyModifierEvent;

        private readonly SingleLinkedList<ModifierHandler<int>> AddersList =
            new SingleLinkedList<ModifierHandler<int>>() { };
        private readonly SingleLinkedList<ModifierHandler<int>> MultipliesList =
            new SingleLinkedList<ModifierHandler<int>>() { };

        protected override void RecalculationAction()
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
        }
        public IConstModifier<int> AddModifier_Add(int modifier)
        {
            return AddModifier(this, modifier, AddersList, (item) => AddingAddModifierEvent?.Invoke(item));
        }
        public IConstModifier<int> AddModifier_Multiply(int modifier)
        {
            return AddModifier(this, modifier, MultipliesList, (item) => AddingMultiplyModifierEvent?.Invoke(item));
        }
    }
}
