using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Messaging;
using Prism.Events;

namespace SpaceTrader
{
    /// <summary>
    /// Static EventAggregator, Built by Rachel Lim. Used without permission as everything is still in house. 
    /// When published, either ask permission or rewrite everything.
    /// </summary>

    //prototypes of messages that can be send between viewmodels

    public class TickerSymbolGalaxyGenerationSettings
    {
        public IGalaxyGenerationSettings GalaxyGenerationSettings { get; set; }
    }

    public class TickerSymbolSelectedMessage
    {
        public string MessageString { get; set; }
    }

    public class TickerSymbolTotalAmountofFoodandPopulation
    {
        public string SpoiledFoodperTurn { get; set; }
        public string DeathsthisTurn { get; set; }
        public string BirthsperTurn { get; set; }
        public string NewcomersperTurn { get; set; }
        public string ProducedFoodperTurn { get; set; }
        public string TotalFoodEndofTurn { get; set; }
        public string TotalPopulationEndofTurn { get; set; }
    }
    //actual eventsystem class.  it uses prism's 'prism.events' namespace
    public static class EventSystem
    {
        private static IEventAggregator _current;
        public static IEventAggregator Current
        {
            get
            {
                return _current ?? (_current = new EventAggregator());
            }
        }

        private static PubSubEvent<TEvent> GetEvent<TEvent>()
        {
            return Current.GetEvent<PubSubEvent<TEvent>>();
        }

        public static void Publish<TEvent>()
        {
            Publish<TEvent>(default);
        }

        public static void Publish<TEvent>(TEvent @event)
        {
            GetEvent<TEvent>().Publish(@event);
        }

        public static SubscriptionToken Subscribe<TEvent>(Action action, ThreadOption threadOption = ThreadOption.PublisherThread, bool keepSubscriberReferenceAlive = false)
        {
            return Subscribe<TEvent>(e => action(), threadOption, keepSubscriberReferenceAlive);
        }

        public static SubscriptionToken Subscribe<TEvent>(Action<TEvent> action, ThreadOption threadOption = ThreadOption.PublisherThread, bool keepSubscriberReferenceAlive = false, Predicate<TEvent> filter = null)
        {
            return GetEvent<TEvent>().Subscribe(action, threadOption, keepSubscriberReferenceAlive, filter);
        }

        public static void Unsubscribe<TEvent>(SubscriptionToken token)
        {
            GetEvent<TEvent>().Unsubscribe(token);
        }
        public static void Unsubscribe<TEvent>(Action<TEvent> subscriber)
        {
            GetEvent<TEvent>().Unsubscribe(subscriber);
        }
    }
}
