using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Events;

namespace Kryz.CharacterStats
{
	[Serializable]
	public class CharacterStat
	{
		public float BaseValue;
		private UnityEvent ChangeEvent;

		protected bool isDirty = true;
		protected float lastBaseValue;

		protected float _value;
		public virtual float Value {
			get {
				if(isDirty || lastBaseValue != BaseValue) {
					lastBaseValue = BaseValue;
					_value = CalculateFinalValue();
					isDirty = false;
				}
				return _value;
			}
		}

		protected readonly List<StatModifier> statModifiers;
		public readonly ReadOnlyCollection<StatModifier> StatModifiers;

		public CharacterStat()
		{
			statModifiers = new List<StatModifier>();
			StatModifiers = statModifiers.AsReadOnly();
		}

		public CharacterStat(float baseValue) : this()
		{
			BaseValue = baseValue;
		}
		public CharacterStat(float baseValue, UnityEvent newAction) : this()
		{
			BaseValue = baseValue;
			ChangeEvent = newAction;
		}

		public virtual void AddModifier(StatModifier mod)
		{
			isDirty = true;
			statModifiers.Add(mod);
		}

		public virtual void AddModifierWithEvent(StatModifier mod)
		{
			isDirty = true;
			statModifiers.Add(mod);
			ChangeEvent?.Invoke();
		}

		public virtual bool RemoveModifier(StatModifier mod)
		{
			if (statModifiers.Remove(mod))
			{
				isDirty = true;
				return true;
			}
			return false;
		}
		public virtual bool RemoveModifierWithEvent(StatModifier mod)
		{
			if (statModifiers.Remove(mod))
			{
				isDirty = true;
				ChangeEvent?.Invoke();
				return true;
			}
			return false;
		}

		public virtual bool RemoveAllModifiersFromSource(object source)
		{
			int numRemovals = statModifiers.RemoveAll(mod => mod.Source == source);

			if (numRemovals > 0)
			{
				isDirty = true;
				return true;
			}
			return false;
		}

		protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
		{
			if (a.Order < b.Order)
				return -1;
			else if (a.Order > b.Order)
				return 1;
			return 0; //if (a.Order == b.Order)
		}
		
		protected virtual float CalculateFinalValue()
		{
			float finalValue = BaseValue;
			float sumPercentAdd = 0;

			statModifiers.Sort(CompareModifierOrder);

			for (int i = 0; i < statModifiers.Count; i++)
			{
				StatModifier mod = statModifiers[i];

				if (mod.Type == StatModType.Flat)
				{
					finalValue += mod.Value;
				}
				else if (mod.Type == StatModType.PercentAdd)
				{
					sumPercentAdd += mod.Value;

					if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.PercentAdd)
					{
						finalValue *= 1 + sumPercentAdd;
						sumPercentAdd = 0;
					}
				}
				else if (mod.Type == StatModType.PercentMult)
				{
					finalValue *= 1 + mod.Value;
				}
			}

			// Workaround for float calculation errors, like displaying 12.00001 instead of 12
			return (float)Math.Round(finalValue, 4);
		}
	}
}
