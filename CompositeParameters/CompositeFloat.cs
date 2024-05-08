using MuonhoryoLibrary.Collections;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuonhoryoLibrary
{
    [Serializable]
    public sealed class CompositeFloat:CompositeParameter<float>
    {
        public CompositeFloat(float DefaultValue):base(DefaultValue) { }

        public event Action<IConstModifier<float>> AddingAddModifierEvent;
        public event Action<IConstModifier<float>> AddingMultiplyModifierEvent;

        private readonly SingleLinkedList<ModifierHandler<float>> AddersList =
            new SingleLinkedList<ModifierHandler<float>>() { };
        private readonly SingleLinkedList<ModifierHandler<float>> MultipliesList = 
            new SingleLinkedList<ModifierHandler<float>>() { };

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

        public IConstModifier<float> AddModifier_Add(float modifier)
        {
            return AddModifier(this, modifier, AddersList, (item) => AddingAddModifierEvent?.Invoke(item));
        }
        public IConstModifier<float> AddModifier_Multiply(float modifer)
        {
            return AddModifier(this, modifer, MultipliesList, (item) => AddingMultiplyModifierEvent?.Invoke(item));
        }
    }
}
