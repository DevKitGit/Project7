using System;
using System.Reflection;
using Devkit.Modularis.Helpers;
using Devkit.Modularis.Variables;
using UnityEngine;

namespace Devkit.Modularis.References
{
    [Serializable]
    public abstract class BaseReference : object
    {
        public abstract void RegisterCallback(Action action);
        public abstract void UnregisterCallback(Action action);
        public abstract void ResetConstant();
        public abstract object GetConstant();

        public Action ConstantValueChanged;
    }

    
    [Serializable]
    public abstract class BaseReference<TVariableType, TBaseType> : BaseReference where TVariableType : BaseVariable<TBaseType>
    {
        public bool useConstant;
        [SerializeField,HideInInspector] private bool foldout;
        public TBaseType constantValue;

        [SerializeField] private TVariableType variable;
        public BaseReference()
        { }

        public override void ResetConstant()
        {
            constantValue = Activator.CreateInstance<TBaseType>();
        }
        
        public override object GetConstant()
        {
            return constantValue;
        }



        public BaseReference(TBaseType value)
        {
            useConstant = true;
            ConstantValue = value;
        }
        
        public static implicit operator TBaseType(BaseReference<TVariableType,TBaseType> reference)
        {
            return reference.Value;
        }

        public TBaseType Value
        {
            get => useConstant ? ConstantValue : Variable.Value;
            set => variable.Value = value;
        }

        public TBaseType ConstantValue
        {
            get => constantValue;
            set
            {
                constantValue = value;
                ConstantValueChanged?.Invoke();
            }
        }
        public TVariableType Variable
        {
            get => variable;
            set => variable = value;
        }

        public override void RegisterCallback(Action action)
        {
            ConstantValueChanged += action;
            if (Variable == null) return;
            Variable.ValueChangeCallback += action;
        }
        
        public override void UnregisterCallback(Action action)
        {
            ConstantValueChanged += action;
            if (Variable == null) return;
            Variable.ValueChangeCallback -= action;
        }
    }
}
