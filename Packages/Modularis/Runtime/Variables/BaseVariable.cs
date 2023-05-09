using System;
using Devkit.Modularis.Helpers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Devkit.Modularis.Variables
{
    [Serializable]
    public abstract class BaseVariable : ScriptableObject
    {
        public abstract object GetValue();

        public abstract void SetValue(object o);
        public abstract void InvokeCallback();
        public abstract void StoreOnEnterPlay();
        public abstract void ResetOnExitPlay(bool reset);
    }

    [Serializable]
    public abstract class BaseVariable<T> : BaseVariable, IResetOnExitPlay
    {
#if UNITY_EDITOR
        [Multiline]
        public string developerDescription = "";
#endif
        [SerializeField] private T value;
        [SerializeField, HideInInspector] private T backupValue;

        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                ValueChangeCallback?.Invoke();
            }
        }
        public override object GetValue()
        {
            return value;
        }
        
        public override void SetValue(object o)
        {
            value = (T)o;
        }


        public Action ValueChangeCallback;
        
        [SerializeField] private bool resetOnGameExit = true;

        public bool ShouldResetOnExitPlay()
        {
            return resetOnGameExit;
        }

        public override void InvokeCallback()
        {
            ValueChangeCallback?.Invoke();
        }
        public override void StoreOnEnterPlay()
        {
            backupValue = value;
        }

        public override void ResetOnExitPlay(bool reset)
        {
            if (!reset) return;
            value = backupValue;
        }
    }
}