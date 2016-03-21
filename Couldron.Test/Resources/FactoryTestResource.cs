namespace Couldron.Test.Resources
{
    public interface IAnimal
    {
        string Name { get; }
    }

    public abstract class Bird : IAnimal
    {
        public abstract string Name { get; }
    }

    public class Cat : IAnimal
    {
        public string Name { get { return this.GetType().Name; } }
    }

    [Factory(typeof(IAnimal))]
    public class Dog : IAnimal
    {
        public string Name { get { return this.GetType().Name; } }
    }

    [Factory(typeof(IAnimal))]
    public class Leopard : IAnimal
    {
        public string Name { get { return this.GetType().Name; } }
    }

    [Factory(typeof(IAnimal))]
    public class Parrot : Bird
    {
        public override string Name { get { return this.GetType().Name; } }
    }

    [Factory(typeof(IAnimal))]
    public class Pigeon : Bird
    {
        public override string Name { get { return this.GetType().Name; } }
    }
}