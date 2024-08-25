


using MuonhoryoLibrary.Collections;
using System;

namespace MuonhoryoLibrary
{
    public abstract class CompositeParameter
    {
        public interface IConstModifier<TParam>
        {
            public void RemoveModifier();
            public TParam Modifier { get; }
        }
        protected sealed class ModifierHandler<TModType> : IConstModifier<TModType>
        {
            public ModifierHandler(TModType Modifier, SingleLinkedList<ModifierHandler<TModType>> list,
                CompositeParameter owner)
            {
                RemoveHandlerAction = () =>
                {
                    list.Remove(this);
                    owner.RecalculateValue();
                };
                this.Modifier = Modifier;
            }
            public readonly TModType Modifier;
            private readonly Action RemoveHandlerAction;
            TModType IConstModifier<TModType>.Modifier => Modifier;
            public void RemoveModifier() => RemoveHandlerAction();
        }
        protected CompositeParameter() { }

        protected abstract void RecalculateValue();
    }
    /// <summary>
    /// Default value changed by add's and multiply's modifiers
    /// </summary>
    public abstract class CompositeParameter<TParamType>:CompositeParameter
    {
        public CompositeParameter(TParamType DefaultValue) : base()
        {
            this.DefaultValue = DefaultValue;
            RecalculateValue();
        }
        public readonly TParamType DefaultValue;
        public TParamType CurrentValue { get; protected set; }
        public event Action<TParamType> ValueHasBeenRecalculatedEvent;
        protected sealed override void RecalculateValue()
        {
            RecalculationAction();
            ValueHasBeenRecalculatedEvent?.Invoke(CurrentValue);
        }
        protected abstract void RecalculationAction();

        protected static IConstModifier<TListParamsType> AddModifier<TOwnerParamType, TListParamsType>(
            CompositeParameter<TOwnerParamType> owner,
            TListParamsType modifierValue,
            SingleLinkedList<ModifierHandler<TListParamsType>> list,
            Action<IConstModifier<TListParamsType>> runningEventAction)
        {
            ModifierHandler<TListParamsType> modifier =
                new ModifierHandler<TListParamsType>(modifierValue, list,owner);
            list.AddLast(modifier);
            runningEventAction(modifier);
            owner.RecalculateValue();
            return modifier;
        }

        public static explicit operator TParamType(CompositeParameter<TParamType>  i) => i.CurrentValue;
    }
}
