using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    public static class INotifyPropertyChangedExtensions
    {
        /// <summary>
        /// Checks if the <paramref name="notificationObject"/> would raise changes for a property named <paramref name="actionDisplayName"/> when the <paramref name="changeAction"/> is invoked.
        /// If the set of the raised notifications does not equal <paramref name="expectedNotifications"/> - <see cref="Assert.Fail"/> would be raised with proper message.
        /// </summary>
        /// <typeparam name="T">A type implementing <see cref="INotifyPropertyChanged"/> to be tested.</typeparam>
        /// <param name="notificationObject">The tested notification object instance.</param>
        /// <param name="actionDisplayName">The display friendly name of the action or properties set in <paramref name="changeAction"/>.</param>
        /// <param name="changeAction">Action to be invoked while the object is being observed.</param>
        /// <param name="expectedNotifications">Which notifications should be throw.</param>
        public static void AssertPropertyChanged<T>(this T notificationObject, string actionDisplayName, Action changeAction, params string[] expectedNotifications)
            where T : INotifyPropertyChanged
        {
            IList<string> receivedNotifications = new List<string>();
            PropertyChangedEventHandler handler = (object s, PropertyChangedEventArgs e) =>
            {
                receivedNotifications.Add(e.PropertyName);
            };
            notificationObject.PropertyChanged += handler;
            changeAction();
            notificationObject.PropertyChanged -= handler;

            if (!receivedNotifications.ItemsEqual(expectedNotifications))
            {
                Assert.Fail("{0}. Notifications expected: {1}. Actual: {2}.", actionDisplayName, string.Join(", ", expectedNotifications), string.Join(", ", receivedNotifications));
            }
        }

        public static Expression<T> Lambda<T>(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            return Expression<T>.Lambda<T>(body, parameters);
        }

        public static void AssertEventFired(this object source, string eventName, Action changeAction, bool shouldFire = true)
        {
            EventInfo eventInfo = source.GetType().GetRuntimeEvent(eventName);
            Assert.IsNotNull(eventInfo, eventName + " event cannot be found.");
            var methodInfo = GenericEventHandler.MethodInfo;
            Assert.IsNotNull(methodInfo, "Unexpected null methodInfo");

            ParameterExpression senderParameter = Expression.Parameter(typeof(object), "sender");

            var eventArgsType = eventInfo.EventHandlerType.GetRuntimeMethods().First(mi => mi.Name == "Invoke").GetParameters()[1].ParameterType;
            ParameterExpression eventArgsParameter = Expression.Parameter(eventArgsType, "e");

            GenericEventHandler handler = new GenericEventHandler();
            var instance = Expression.Constant(handler);

            MethodCallExpression eventFiredExpression = Expression.Call(instance, methodInfo, senderParameter, eventArgsParameter);

            LambdaExpression lambda = Expression.Lambda(eventInfo.EventHandlerType, eventFiredExpression, senderParameter, eventArgsParameter);

            var eventHandler = lambda.Compile();

            eventInfo.AddEventHandler(source, eventHandler);
            changeAction();
            eventInfo.RemoveEventHandler(source, eventHandler);

            if (shouldFire)
            {
                Assert.IsTrue(handler.EventFired, eventName + " did not Fired.");
            }
            else
            {
                Assert.IsFalse(handler.EventFired, eventName + " did Fired Incorrectly.");
            }
        }

        private class GenericEventHandler
        {
            public static MethodInfo MethodInfo = typeof(GenericEventHandler).GetRuntimeMethods().First(mi => mi.Name =="OnEventTriggered");

            public bool EventFired { get; private set; }

            private void OnEventTriggered(object sender, EventArgs e)
            {
                this.EventFired = true;
            }
        }
    }
}