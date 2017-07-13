using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Abstract factory is usefull to build up hirarchy of factories.
/// </summary>
namespace DesignPatterns.Creational.Factory
{
    public interface IHotDrink
    {
        void Consume();
    }

    internal class Tea : IHotDrink
    {
        public void Consume()
        {
            Console.WriteLine("This tea is nice but I'd prefer it with milk.");
        }
    }

    internal class Coffee : IHotDrink
    {
        public void Consume()
        {
            Console.WriteLine("This coffee is delicious!");
        }
    }

    public interface IHotDrinkFactory
    {
        IHotDrink Prepare(int amount);
    }

    internal class TeaFactory : IHotDrinkFactory
    {
        public IHotDrink Prepare(int amount)
        {
            Console.WriteLine($"Put in tea bag, boil water, pour {amount} ml, add lemon, enjoy!");
            return new Tea();
        }
    }

    internal class CoffeeFactory : IHotDrinkFactory
    {
        public IHotDrink Prepare(int amount)
        {
            Console.WriteLine($"Grind some beans, boil water, pour {amount} ml, add cream and sugar, enjoy!");
            return new Coffee();
        }
    }

    public class HotDrinkMachine
    {
        private List<Tuple<string, IHotDrinkFactory>> namedFactories =
          new List<Tuple<string, IHotDrinkFactory>>();

        public HotDrinkMachine()
        {
            // Use reflection to fullfill the open closed principle -> here no code change is required if
            // a new hotdrink category is added. Alternativelly to reflection you can use an IoC container!
            foreach (var t in typeof(HotDrinkMachine).Assembly.GetTypes())
            {
                if (typeof(IHotDrinkFactory).IsAssignableFrom(t) && !t.IsInterface)
                {
                    namedFactories.Add(Tuple.Create(
                      t.Name.Replace("Factory", string.Empty), (IHotDrinkFactory)Activator.CreateInstance(t)));
                }
            }
        }

        public IHotDrink MakeDrink(string drinkType)
        {
            Console.WriteLine("Available drinks");
            for (var index = 0; index < namedFactories.Count; index++)
            {
                var tuple = namedFactories[index];
                Console.WriteLine($"{index}: {tuple.Item1}");
            }

            var fac = namedFactories.Where(tuple => tuple.Item1.Equals(drinkType)).Select(tuple => tuple.Item2).Single();

            return fac?.Prepare(100) ?? null;
        }
    }

    [TestFixture]
    public class AbstractFactoryTest
    {

        [Test]
        public void GetTea()
        {
            var machine = new HotDrinkMachine();

            IHotDrink drink = machine.MakeDrink("Tea");
            Assert.IsNotNull(drink);
            Assert.IsNotNull(drink as Tea);
        }
    }
}
