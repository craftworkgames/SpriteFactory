using System;
using System.Linq.Expressions;
using System.Reflection;
using Catel.MVVM;

namespace SpriteFactory
{
    public class ViewModel : ViewModelBase
    {
        protected bool SetPropertyValue<T>(ref T backingStore, T value, Expression<Func<T>> propertyExpression)
        {
            if (!Equals(backingStore, value))
            {
                backingStore = value;
                RaisePropertyChanged(propertyExpression);
                return true;
            }

            return false;
        }

        protected bool SetPropertyValue<T>(ref T backingStore, T value, string nameOfProperty)
        {
            if (!Equals(backingStore, value))
            {
                backingStore = value;
                RaisePropertyChanged(nameOfProperty);
                return true;
            }

            return false;
        }

        protected bool SetPropertyValue<TTarget, T>(TTarget target, Expression<Func<TTarget, T>> outExpr, T value, string nameOfProperty)
        {
            var expr = (MemberExpression)outExpr.Body;
            var propertyInfo = (PropertyInfo)expr.Member;

            if (!Equals(propertyInfo.GetValue(target), value))
            {
                propertyInfo.SetValue(target, value);
                RaisePropertyChanged(nameOfProperty);
                return true;
            }

            return false;
        }
    }
}