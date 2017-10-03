﻿using System;

namespace DotNet.Basics.Sys
{
    public class Observable<T>
    {
        private T _value;

        public delegate void ValueChangeEventHandler(T args);

        public event ValueChangeEventHandler Updating;
        public event ValueChangeEventHandler Updated;

        public Observable()
        {
            var type = typeof(T);
            bool isPrimitive = type.IsPrimitive;
            _notifyListeners = true;
            if (isPrimitive)
                _setValue = ValueTypeSetter;
            else
                _setValue = ReferenceTypeSetter;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        private bool _notifyListeners;

        private readonly Func<T, bool> _setValue;


        public void Reset()
        {
            _notifyListeners = false;
            Value = default(T);
            _notifyListeners = true;
        }

        public T Value
        {
            get { return _value; }
            set { _setValue(value); }
        }

        private void SetValue(T t)
        {
            if (_notifyListeners)
                Updating?.Invoke(_value);

            _value = t;

            if (_notifyListeners)
                Updated?.Invoke(_value);
        }

        private bool ValueTypeSetter(T t)
        {
            if (_value.Equals(t))
                return false;
            SetValue(t);
            return true;
        }

        private bool ReferenceTypeSetter(T t)
        {
            //it will never be value type since this was decided in constructor
            if (_value == null && t == null)
                return false;

            if (_value != null && t != null && _value.Equals(t))
                return false;

            SetValue(t);
            return true;
        }
    }
}