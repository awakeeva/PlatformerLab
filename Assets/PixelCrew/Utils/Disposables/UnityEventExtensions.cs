using System;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew.Utils.Disposables
{
    public static class UnityEventExtensions
    {
        public static IDisposable Subscribe(this UnityEvent unityEvent, UnityAction call)
        {
            unityEvent.AddListener(call);

            return
                new ActionDisposable(() => unityEvent.RemoveListener(call));
        }

        public static IDisposable Subscribe<TType>(this UnityEvent<TType> unityEvent, UnityAction<TType> call)
        {
            unityEvent.AddListener(call);

            return
                new ActionDisposable(() => unityEvent.RemoveListener(call));
        }

        public static IDisposable Subscribe<TType, TType2>(this UnityEvent<TType, TType2> unityEvent, UnityAction<TType, TType2> call)
        {
            unityEvent.AddListener(call);

            return
                new ActionDisposable(() => unityEvent.RemoveListener(call));
        }


    }
}

